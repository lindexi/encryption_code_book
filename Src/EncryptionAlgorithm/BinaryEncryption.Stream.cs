#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Lindexi.Src.EncryptionAlgorithm
{
    static partial class BinaryEncryption
    {
        /// <summary>
        /// 二进制流加密配置
        /// </summary>
        static class StreamBinaryEncryptionConfiguration
        {
            /// <summary>
            /// 缓冲区的长度
            /// </summary>
            public const int BufferLength = 1024;

            /// <summary>
            /// 密码块的 Byte 长度
            /// </summary>
            /// 生成加密块，都是追加在明文之后，尺寸取更小。第一次取 512 为 1024 一半，为明文建议长度。后续再除以 4 表示取更小比例
            public const int KeyBlockByteLength =
                    BufferLength / 2 / 4;

            /// <summary>
            /// 一个 int 等于几个 byte 长度
            /// </summary>
            public const int ByteCountOfInt = 4;

            /// <summary>
            /// 密码块的 Int 长度
            /// </summary>
            public const int KeyBlockIntLength = KeyBlockByteLength / ByteCountOfInt;

            /// <summary>
            /// 哈希的 Byte 长度
            /// </summary>
            /// 这里是 MD5 的默认长度
            public const int HashByteLength = 16;
        }

        /// <summary>
        /// 对传入的 <paramref name="inputStream"/> 进行加密，然后写入到 <paramref name="outputStream"/> 里
        /// </summary>
        /// <param name="inputStream">明文</param>
        /// <param name="outputStream">密文</param>
        /// <param name="key">密码</param>
        /// <param name="settings">加密的设置。如传入加密设置时，解密过程应该传入相等的加密设置</param>
        public static void EncryptStream(System.IO.Stream inputStream, System.IO.Stream outputStream, int[] key,
            EncryptStreamSettings? settings = null)
        {
            Random random
#if NET6_0_OR_GREATER
                = Random.Shared;
#else
                = new Random();
#endif

            // 是否应该追加哈希到密码块
            var shouldAppendHashToKeyBlock = settings?.ShouldAppendHashToKeyBlock ?? true;

            const int bufferLength = StreamBinaryEncryptionConfiguration.BufferLength;

            const int keyBlockByteLength = StreamBinaryEncryptionConfiguration.KeyBlockByteLength;
            const int byteCountOfInt = StreamBinaryEncryptionConfiguration.ByteCountOfInt;

            const int keyBlockIntLength = StreamBinaryEncryptionConfiguration.KeyBlockIntLength;

            using var hash = MD5.Create();
            const int hashByteLength = StreamBinaryEncryptionConfiguration.HashByteLength; // 这是 md5 的默认固定长度

            // 只有首个加密块使用了输入的密码信息，后续的明文块都采用加密块的加密数据
            var keyBlock = new int[keyBlockIntLength];
            var currentKeyBlock = new int[keyBlockIntLength];

            var inputBuffer = ArrayPool<byte>.Shared.Rent(bufferLength);
            var outputBuffer = ArrayPool<byte>.Shared.Rent(bufferLength);
            var tempHashBuffer = outputBuffer;
            try
            {
                // 具体步骤是：
                // 1. 创建加密块，加密块里面将生成加密的密码数据
                // 2. 读取输入流，每次读取出块，然后加密，加密内容由加密块提供，然后写入输出流

                // 首段特殊处理，包含校验密码的功能
                // 校验密码方式是创建 MD5 哈希，测试其解密是否正确
                FillKeyBlockData(inputBuffer.AsSpan(0, keyBlockByteLength), keyBlock, random);

                var currentInputBufferLength = keyBlockByteLength;

                if (shouldAppendHashToKeyBlock)
                {
                    // 计算用于拼接的 MD5 值
                    var computeHashResult = hash.TryComputeHash(inputBuffer.AsSpan().Slice(0, keyBlockByteLength),
                        tempHashBuffer.AsSpan(), out var hashWrittenByteLength);
                    Debug.Assert(computeHashResult is true);
                    Debug.Assert(hashWrittenByteLength == hashByteLength);
                    // 追加到输入里
                    tempHashBuffer.AsSpan(0, hashByteLength)
                        .CopyTo(inputBuffer.AsSpan(keyBlockByteLength));
                    currentInputBufferLength = currentInputBufferLength + hashByteLength;
                }
                else
                {
                    // 不追加哈希值
                }

                // 再填充一些干扰数据项。因为 当前已用长度 必然是一个固定值，太好猜长度了，多加点垃圾数据，伤害一下攻击者心智
                // 现在可填充的为剩余空间减去当前已用长度
                var fillLength = bufferLength - currentInputBufferLength;
                fillLength = random.Next(fillLength);
                random.NextBytes(inputBuffer.AsSpan(currentInputBufferLength, fillLength));
                currentInputBufferLength = currentInputBufferLength + fillLength;

                // 将其加密后写入到输出里
                EncryptData_1_1_2(inputBuffer, 0, currentInputBufferLength, key, outputBuffer, bufferLength, random);
                outputStream.Write(outputBuffer, 0, bufferLength);

                // 将密码块复制出来作为当前密码块
                keyBlock.CopyTo(currentKeyBlock.AsSpan());

                // 开始写入对明文进行加密后的密文
                while (true)
                {
                    // 读取的长度为固定长加上随机的长度，但随机长度是受控制的，确保读取是在缓冲区的一半附近
                    var inputLength = bufferLength / 4 + random.Next(bufferLength / 4);

//#if DEBUG
//                    inputLength = bufferLength / 4;
//#endif

                    var readLength = inputStream.Read(inputBuffer, 0, inputLength);
                    if (readLength == 0)
                    {
                        // 全部读取完了
                        break;
                    }

                    // 即使最后一段，也给创建下一段的密码块，不通过 Stream 读取返回内容判断是否最后一段
                    //var needCreateNextKeyBlock = readLength == inputLength;

                    currentInputBufferLength = readLength;

                    // 生成下一段的加密块
                    FillKeyBlockData(inputBuffer.AsSpan(currentInputBufferLength, keyBlockByteLength), keyBlock,
                        random);
                   
                    currentInputBufferLength = currentInputBufferLength + keyBlockByteLength;

                    if (shouldAppendHashToKeyBlock)
                    {
                        // 下一段的密码块不需要再计算其 MD5 值了，直接拼接就好了。第一段之所以计算 MD5 只是为了进行校验
                        // 因此无论 shouldAppendHashToKeyBlock 是否为 true 的值，都不需要计算 MD5 值
                    }

                    // 明文块里面的加密都不使用传入的密码，而是采用密码块进行加密。如此可以确保明文是顺序重复的情况下，也能让密文不重复。同时减少传入的密码加密了多次明文之后的统计攻击
                    EncryptData_1_1_2(inputBuffer, 0, currentInputBufferLength, currentKeyBlock, outputBuffer,
                        bufferLength, random);
                    outputStream.Write(outputBuffer, 0, bufferLength);

#if DEBUG
                    // 立刻解密，看是否正确
                    var debugData = new byte[bufferLength];
                    DecryptData_1_1_2(outputBuffer, 0, bufferLength, currentKeyBlock, debugData, bufferLength,
                        out var decryptionResultBufferLength);
                    Debug.Assert(decryptionResultBufferLength == currentInputBufferLength);

                    for (var i = 0; i < decryptionResultBufferLength; i++)
                    {
                        if (debugData[i] != inputBuffer[i])
                        {
                        }
                    }
#endif

                    // 将密码块复制出来作为当前密码块，给下一段使用
                    keyBlock.CopyTo(currentKeyBlock.AsSpan());
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(inputBuffer);
                ArrayPool<byte>.Shared.Return(outputBuffer);
            }
        }

        private static void FillKeyBlockData(Span<byte> keyBlockBuffer, int[] keyBlock, Random random)
        {
            const int byteCountOfInt = StreamBinaryEncryptionConfiguration.ByteCountOfInt;

            const int keyBlockIntLength = StreamBinaryEncryptionConfiguration.KeyBlockIntLength;

            random.NextBytes(keyBlockBuffer);
//#if DEBUG
//            for (int i = 0; i < keyBlockBuffer.Length; i++)
//            {
//                keyBlockBuffer[i] = (byte) i;
//            }
//#endif
            for (int i = 0; i < keyBlockIntLength; i++)
            {
                var keyValue = BitConverter.ToInt32(keyBlockBuffer.Slice(byteCountOfInt * i));
                // 密码只取正数，因为加密模块里面对负数计算不正确，等后续修复了，再允许密码为负数
                keyBlock[i] = Math.Abs(keyValue);
            }
        }

        /// <summary>
        /// 尝试使用 <paramref name="key"/> 解密 <paramref name="inputStream"/>，然后写入到 <paramref name="outputStream"/> 里
        /// </summary>
        /// <param name="inputStream">密文</param>
        /// <param name="outputStream">明文</param>
        /// <param name="key"></param>
        /// <param name="settings">加密的设置。解密过程中需要传入和加密相等的配置</param>
        /// <returns>True: 解密成功；False: 解密失败，密码错误、或数据错误</returns>
        public static bool TryDecryptStream(System.IO.Stream inputStream, System.IO.Stream outputStream, int[] key,
            EncryptStreamSettings? settings = null)
        {
            // 是否应该追加哈希到密码块
            var shouldAppendHashToKeyBlock = settings?.ShouldAppendHashToKeyBlock ?? true;

            const int bufferLength = StreamBinaryEncryptionConfiguration.BufferLength;

            const int keyBlockByteLength = StreamBinaryEncryptionConfiguration.KeyBlockByteLength;
            const int byteCountOfInt = StreamBinaryEncryptionConfiguration.ByteCountOfInt;

            const int keyBlockIntLength = StreamBinaryEncryptionConfiguration.KeyBlockIntLength;

            using var hash = MD5.Create();
            const int hashByteLength = StreamBinaryEncryptionConfiguration.HashByteLength;

            // 只有首个加密块使用了输入的密码信息，后续的明文块都采用加密块的加密数据
            var currentKeyBlock = new int[keyBlockIntLength];

            var inputBuffer = ArrayPool<byte>.Shared.Rent(bufferLength);
            var outputBuffer = ArrayPool<byte>.Shared.Rent(bufferLength);
            try
            {
                // 先读取第一个块，然后解密，判断其校验是否正确，如果不正确，则证明密码错误
                // 先读取第一块。如果读取的长度不足，则说明数据错误
                var readLength = inputStream.Read(inputBuffer, 0, bufferLength);
                if (readLength != bufferLength)
                {
                    // 长度不够，则证明数据错误
                    return false;
                }

                // 解密第一个块
                DecryptData_1_1_2(inputBuffer, 0, bufferLength, key, outputBuffer, bufferLength, out var decryptionResultBufferLength);

                if (shouldAppendHashToKeyBlock)
                {
                    // 在有追加哈希时，应该校验哈希是否正确
                    if (decryptionResultBufferLength < keyBlockByteLength + hashByteLength)
                    {
                        // 解密出来的长度太小了，证明数据错误或密码错误
                        return false;
                    }

                    // 取输出的部分内容作为哈希的缓冲，减少一次多余的内存分配
                    var tempHashBuffer = outputBuffer.AsSpan(keyBlockByteLength + hashByteLength);
                    var computeHashResult = hash.TryComputeHash(outputBuffer.AsSpan(0, keyBlockByteLength), tempHashBuffer,
                        out var hashWrittenByteLength);
                    Debug.Assert(computeHashResult is true);
                    Debug.Assert(hashWrittenByteLength == hashByteLength);
                    // 存放在密文里面的哈希，看是否和解密出的哈希一致
                    var keyBlockHash = outputBuffer.AsSpan(keyBlockByteLength, hashByteLength);
                    for (int i = 0; i < hashByteLength; i++)
                    {
                        if (tempHashBuffer[i] != keyBlockHash[i])
                        {
                            // 哈希不一致，证明密码错误
                            return false;
                        }
                    }
                }
                else
                {
                    // 不追加哈希值，直接判断长度是否正确
                    if (decryptionResultBufferLength < keyBlockByteLength)
                    {
                        // 解密出来的长度太小了，证明数据错误或密码错误
                        return false;
                    }
                    // 为什么不是等于？因为在前面加密过程中，追加了一些垃圾数据，所以解密出来的长度会比原来的长
                }

                // 装入到当前的密码块
                for (int i = 0; i < keyBlockIntLength; i++)
                {
                    var keyValue = BitConverter.ToInt32(outputBuffer, byteCountOfInt * i);
                    currentKeyBlock[i] = Math.Abs(keyValue);
                }

//#if DEBUG
//                Array.Clear(outputBuffer, 0, outputBuffer.Length);
//#endif

                // 开始解密后续的块
                while (true)
                {
                    readLength = inputStream.Read(inputBuffer, 0, bufferLength);
                    if (readLength == 0)
                    {
                        // 读取完成了，证明解密完成
                        break;
                    }

                    if (readLength < bufferLength)
                    {
                        // 长度不够，则证明数据错误
                        return false;
                    }

                    // 解密
                    DecryptData_1_1_2(inputBuffer, 0, bufferLength, currentKeyBlock, outputBuffer, bufferLength,
                        out decryptionResultBufferLength);
                    var dataLength = decryptionResultBufferLength;
                    if (dataLength > keyBlockByteLength)
                    {
                        dataLength = decryptionResultBufferLength - keyBlockByteLength;
                        // 证明后续是有部分是密码块，需要更新密码块
                        // 从明文数据之后的就是密码块
                        var start = dataLength;
                        for (int i = 0; i < keyBlockIntLength; i++)
                        {
                            var keyValue = BitConverter.ToInt32(outputBuffer, start + byteCountOfInt * i);
                            currentKeyBlock[i] = Math.Abs(keyValue);
                        }
                    }
                    // 写入到输出
                    outputStream.Write(outputBuffer, 0, dataLength);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(inputBuffer);
                ArrayPool<byte>.Shared.Return(outputBuffer); 
            }

            return true;
        }
    }
}

#endif
