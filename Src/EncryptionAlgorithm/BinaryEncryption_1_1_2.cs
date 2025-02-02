﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lindexi.Src.EncryptionAlgorithm
{
    // 本文件存放的是修复了 dataValue = (byte) (dataValue + hashData.KeyValue); 将构成哈希下标的密码加入了密文计算里面的版本。由于一旦改了此版本内容会导致之前加密的内容无法正确解密，于是就升级了此版本，再写一套好了
    // 在 1.1.2 版本算法里面修复了上述问题，使用了密码的下一个字符而不是当前字符参与加密
    // 同时去掉后缀，改成在开头写入其数据长度。去掉后缀是有伙伴告诉我说碰撞测试时，我的算法可以被猜测出来固定后缀对应的地方。虽然人家没真的做出来，但还是听建议去掉
    public static partial class BinaryEncryption
    {
        /// <summary>
        /// 使用 1.1.2 版本的数据加密算法
        /// </summary>
        /// <param name="data">明文数据</param>
        /// <param name="dataStart">数据的起始点</param>
        /// <param name="dataLength">数据的长度。要求数据长度小于 <see cref="bufferLength"/>-4 长度，且最佳加密效果要求是 <see cref="bufferLength"/> 的一半</param>
        /// <param name="key">密码</param>
        /// <param name="outputBuffer">用于承载输出的缓冲区，加密结果将会写入到这里</param>
        /// <param name="bufferLength">缓冲区长度，要求缓冲区长度不大于 <paramref name="outputBuffer"/> 数据长度</param>
        /// <param name="randomNumberGenerator"></param>
        public static void EncryptData_1_1_2(byte[] data, int dataStart, int dataLength, int[] key, byte[] outputBuffer, int bufferLength = 1024, IRandomNumberGenerator? randomNumberGenerator = null)
        {
            if (bufferLength > outputBuffer.Length)
            {
                throw new ArgumentException($"缓冲区数据的长度比缓冲区长度小 bufferLength({bufferLength}) > outputBuffer.Length({outputBuffer.Length})");
            }

            if (data.Length < dataStart + dataLength)
            {
                throw new ArgumentException($"data.Length({data.Length}) < dataStart({dataStart}) + dataLength({dataLength})");
            }

            var totalLength = dataLength + SizeofDataLengthInt;
            if (totalLength > bufferLength)
            {
                throw new ArgumentException($"数据长度大于缓冲区长度，必须至少比缓冲区长度小 4 的长度 dataLength({dataLength}) + 4 > bufferLength({bufferLength})");
            }

            randomNumberGenerator ??= new DefaultRandomNumberGenerator();

            var rawDataSpan = new ByteSpan(data, dataStart, dataLength);
            var hashList = new BitArray(bufferLength);
            var context = new EncryptionContext(rawDataSpan, key, hashList, outputBuffer, bufferLength, randomNumberGenerator);
            EncryptDataCore_1_1_2(context);
        }

        private static void EncryptDataCore_1_1_2(EncryptionContext context)
        {
            var dataLength = context.DataLength;
            int nextHashValue = 0;
            for (var i = 0; i < dataLength; i++)
            {
                // 密码位置
                // 需要读取两次，首次读取的是地址，二次读取的是密码。防止地址和密码关联导致的破译
                int hashValue;
                if (i == 0)
                {
                    // 首次读取出地址
                    hashValue = GetPlace_1_1_2(context, i).HashValue;

                    // 需要立刻标记，防止后续再次读取到这个地址
                    context.HashList[hashValue] = true;
                }
                else
                {
                    // 上一个数据已经读取出地址了，就不用再次读取了
                    hashValue = nextHashValue;
                }

                (nextHashValue, var keyData) = GetPlace_1_1_2(context, i + 1);
                // 需要立刻标记，防止后续再次读取到这个地址
                context.HashList[nextHashValue] = true;

                byte dataValue = context.GetData(i);

                // ~~这里算法上存在缺陷，那就是让某个下标的元素的数据耦合了 Key 的数据，让其之间存在关联关系
                // 用人话说就是 HashValue 这个下标是依靠 KeyValue 计算出来的，因此在知道总数是 1024 的情况下，即可推断 HashValue 下标的内容加的可能是多少的值。假定不考虑二圈的情况，只考虑一圈时，如果此时 HashValue 是 2 的值，密文里是 99 的值，那极有可能是 KeyValue 是 2 或 1026 的值，将其减去 2 则得到密码是 97 即 'a' 的值。如此即可大概破解部分内容，甚至为后续更进一步推断提供更多信息
                // 原设计里面，这里应该加的是计算出 HashValue 的后一位密码~~
                // 当前修复此问题，取 nextHashValue 对应的密码作为加密密码，如此即可规避此问题
                // 感谢 @SeWZC https://github.com/lindexi/encryption_code_book/issues/12
                dataValue = (byte) (dataValue + keyData);

                context.Buffer[hashValue] = dataValue;
            }

            // 填充空白部分
            for (var i = 0; i < context.HashList.Length; i++)
            {
                if (!context.HashList[i])
                {
                    context.Buffer[i] = context.RandomNumberGenerator.GenerateFillGapByte();
                    context.HashList[i] = true;
                }
            }
        }

        /// <summary>
        ///     根据传入参数，获取第 <paramref name="index" /> 个坐标
        /// </summary>
        /// <returns></returns>
        private static HashData GetPlace_1_1_2(IContext context, int index)
        {
            int bufferLength = context.BufferLength;

            int[] key = context.Key;
            var keyPlace = index % key.Length;

            // 读取的密码位置，可以计算出字符位置
            int keyValue = key[keyPlace];
            BitArray hashList = context.HashList;

            var hashValue = keyValue % bufferLength;
            if (!hashList[hashValue])
            {
                // 第零次，首次命中，此时不应该使用密码本身，应该选用下一个密码，避免被此计算出来
                // 由于 hashValue 是依靠 keyValue 计算出来的。因此不能再使用这个 keyValue 值。否则拿到对应的位置，即可了解到当前这个位置被 bufferLength 倍的 keyValue 加密，从而可以进行猜测破解
                // 要求上层调用方，不能同时使用相同一次返回的 HashData 的 hashValue 和 keyValue 作为地址和加密，需要再跑一次。即首次获取的是地址，二次获取的是加密密码。二次获取时须将 index 加一
                return new HashData(hashValue, keyValue);
            }

            // 第一圈是使用密码自身
            foreach (var _ in key)
            {
                keyPlace++;
                keyPlace = keyPlace % key.Length;
                keyValue += key[keyPlace] * index;
                // 由于 key[keyPlace] * i 可能存在越界问题，如果越界就会计算出负数，这里需要重新修改为正数
                keyValue = Math.Abs(keyValue);
                hashValue = keyValue % bufferLength;

                if (!hashList[hashValue])
                {
                    return new HashData(hashValue, keyValue);
                }
            }

            // 第二圈是尝试加上明文本身
            const int sizeOfInt = 4;
            const int skipCount = 2;
            if (index - skipCount >= sizeOfInt)
            {
                // 使用 j = i - skipCount 原因是如果是解密的过程，那么当前的明文依然未知
                // 只能取已经解密过的明文来参加计算
                for (var j = index - skipCount; j >= sizeOfInt; j -= sizeOfInt)
                {
                    var byte1 = ReadByte(context, j);
                    var byte2 = ReadByte(context, j - 1);
                    var byte3 = ReadByte(context, j - 2);
                    var byte4 = ReadByte(context, j - 3);

                    var n = (byte1 << (3 * 8))
                            + (byte2 << (2 * 8))
                            + (byte3 << (1 * 8))
                            + byte4;
                    n *= index;
                    keyValue = keyValue ^ n;
                    //if (keyValue < 0)
                    //{
                    //    keyValue *= -1;
                    //}
                    keyValue = Math.Abs(keyValue); // 使用 Math.Abs 会比判断小于零再乘以负一快一点，使用 Math.Abs 指令优化

                    hashValue = keyValue % bufferLength;
                    if (!hashList[hashValue])
                    {
                        return new HashData(hashValue, keyValue);
                    }
                }
            }
            else if (index > 0)
            {
                // 为什么 j = i - 1 原因是如果是解密的过程，那么当前的明文依然未知
                for (var j = index - 1; j >= 0; j--)
                {
                    keyValue += ReadByte(context, j);
                    hashValue = keyValue % bufferLength;

                    if (!hashList[hashValue])
                    {
                        return new HashData(hashValue, keyValue);
                    }
                }
            }

            while (hashList[hashValue])
            {
                // 最后就是 +1 了
                keyValue++;
                hashValue = keyValue % bufferLength;
            }

            return new HashData(hashValue, keyValue);

            static byte ReadByte(IContext context, int index)
            {
                return context.GetData(index);
            }
        }

        private const int SizeofDataLengthInt = 4; // sizeof(int)

        interface IContext
        {
            int[] Key { get; }
            int BufferLength { get; }

            /// <summary>
            /// 和 buffer 构成哈希，用来决定 buffer 上的位置是否被写入值
            /// </summary>
            BitArray HashList { get; }

            /// <summary>
            /// 获取明文数据
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            byte GetData(int index);
        }

        class EncryptionContext : IContext
        {
            public EncryptionContext(ByteSpan rawDataSpan, int[] key, BitArray hashList, byte[] buffer, int bufferLength, IRandomNumberGenerator randomNumberGenerator)
            {
                _rawDataSpan = rawDataSpan;
                Key = key;
                HashList = hashList;
                Buffer = buffer;
                BufferLength = bufferLength;
                RandomNumberGenerator = randomNumberGenerator;
            }

            private readonly ByteSpan _rawDataSpan;
            public int[] Key { get; }
            /// <summary>
            /// 和 buffer 构成哈希，用来决定 buffer 上的位置是否被写入值
            /// </summary>
            public BitArray HashList { get; }
            public byte[] Buffer { get; }
            public int BufferLength { get; }

            public int DataLength => _rawDataSpan.Length + SizeofDataLengthInt;
            public IRandomNumberGenerator RandomNumberGenerator { get; }

            public unsafe byte GetData(int index)
            {
                // 前 4 个 byte 是长度信息
                if (index < SizeofDataLengthInt)
                {
                    // 以下代码等于于以下注释的代码，只是为了兼容更低版本框架，才采用不安全代码
                    // Span<byte> byteSpan = MemoryMarshal.AsBytes(new Span<int>(ref length));

                    var length = _rawDataSpan.Length;
                    /*
                         byte[] bytes = new byte[sizeof(int)];
                                Unsafe.As<byte, int>(ref bytes[0]) = value;
                                return bytes;
                     */
                    int* p = &length;
                    byte* pByte = (byte*) p;
                    return pByte[index];
                }

                var dataIndex = index - SizeofDataLengthInt;
                return _rawDataSpan[dataIndex];
            }
        }

        readonly struct ByteSpan
        {
            public ByteSpan(byte[] data, int start, int length)
            {
                Data = data;
                Start = start;
                Length = length;
            }

            public byte this[int index] => Data[Start + index];
            public byte[] Data { get; }
            public int Start { get; }
            public int Length { get; }
        }

        class DecryptionContext : IContext
        {
            public DecryptionContext(int[] key, int bufferLength, BitArray hashList, byte[] data)
            {
                Key = key;
                BufferLength = bufferLength;
                HashList = hashList;
                Data = data;
            }

            public int[] Key { get; }
            public int BufferLength { get; }
            public BitArray HashList { get; }

            /// <summary>
            /// 数据长度
            /// </summary>
            public int DataLength { get; set; }

            /// <summary>
            /// 明文
            /// </summary>
            public byte[] Data { get; }

            /// <summary>
            /// 当前解密到的点
            /// </summary>
            public int CurrentDecryptIndex { get; set; }

            public unsafe byte GetData(int index)
            {
                // 前 4 个 byte 是长度信息
                if (index < SizeofDataLengthInt)
                {
                    var length = DataLength;
                    /*
                         byte[] bytes = new byte[sizeof(int)];
                                Unsafe.As<byte, int>(ref bytes[0]) = value;
                                return bytes;
                     */
                    int* p = &length;
                    byte* pByte = (byte*) p;
                    return pByte[index];
                }

                var dataIndex = index - SizeofDataLengthInt;
                if (dataIndex > CurrentDecryptIndex)
                {
                    throw new InvalidOperationException($"当前所取的数据超过解密的点");
                }
                return Data[dataIndex];
            }
        }

        /// <summary>
        /// 使用 1.1.2 版本的数据解密算法
        /// </summary>
        /// <param name="encryptionData">加密了的数据</param>
        /// <param name="encryptionDataStart"></param>
        /// <param name="encryptionDataLength"></param>
        /// <param name="key"></param>
        /// <param name="outputBuffer">承载加密输出的缓冲数组</param>
        /// <param name="bufferLength">加密解密过程使用的缓冲数组长度。要求加密解密长度相同</param>
        /// <param name="decryptionResultBufferLength">解密得到的数据长度</param>
        public static void DecryptData_1_1_2(byte[] encryptionData, int encryptionDataStart, int encryptionDataLength, int[] key, byte[] outputBuffer, int bufferLength, out int decryptionResultBufferLength)
        {
            decryptionResultBufferLength = 0;

            var encryptionDataSpan = new ByteSpan(encryptionData, encryptionDataStart, encryptionDataLength);

            var hashList = new BitArray(bufferLength);
            var decryptionContext = new DecryptionContext(key, bufferLength, hashList, outputBuffer);

            // 先解密出数据长度
            int nextHashValue = 0;
            unsafe
            {
                var dataLengthByteList = stackalloc byte[SizeofDataLengthInt];
                for (int i = 0; i < SizeofDataLengthInt; i++)
                {
                    int hashValue;
                    if (i == 0)
                    {
                        hashValue = GetPlace_1_1_2(decryptionContext, i).HashValue;

                        // 需要立刻标记，防止后续再次读取到这个地址
                        hashList[hashValue] = true;
                    }
                    else
                    {
                        hashValue = nextHashValue;
                    }

                    (nextHashValue, var keyData) = GetPlace_1_1_2(decryptionContext, i + 1);
                    // 需要立刻标记，防止后续再次读取到这个地址
                    hashList[nextHashValue] = true;

                    var value = encryptionDataSpan[hashValue];
                    value = (byte) (value - keyData);
                    dataLengthByteList[i] = value;

                }

                decryptionContext.DataLength = *(int*) dataLengthByteList;
            }

            var dataLength = decryptionContext.DataLength;
            if (dataLength < 0)
            {
                // 证明解密失败
                decryptionResultBufferLength = 0;
                return;
            }
            decryptionResultBufferLength = dataLength;

            for (int i = 0; i < dataLength; i++)
            {
                var index = SizeofDataLengthInt + i;
                var hashValue = nextHashValue;
                (nextHashValue, var keyData) = GetPlace_1_1_2(decryptionContext, index + 1);
                hashList[nextHashValue] = true;

                var value = encryptionDataSpan[hashValue];
                value = (byte) (value - keyData);
                outputBuffer[i] = value;

                decryptionContext.CurrentDecryptIndex++;
            }
        }
    }
}
