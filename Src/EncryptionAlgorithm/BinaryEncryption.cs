using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lindexi.Src.EncryptionAlgorithm
{
    public  static partial class BinaryEncryption
    {
        /// <summary>
        ///     分段加密数据，用于数据量特别大的情况。但是不足在于，传入的数据都是相同的数据，那么将会让返回值具有循环，可以被用来删除填补空白的数据。如果数据传入可能考虑攻击者传入数据，可以调用为
        ///     <see cref="EncryptData" /> 方法，通过更底层方式进行加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="bufferLength"></param>
        /// <param name="suffixData"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, int[] key, int bufferLength = 1024,
            byte[]? suffixData = null, Random? random = null)
        {
            suffixData ??= DefaultSuffixData;
            random ??= new Random();
            var maxLength = bufferLength - suffixData.Length;

            if (maxLength <= 2)
            {
                throw new ArgumentException($"{nameof(bufferLength)} must more than {nameof(suffixData)} length");
            }

            if (data.Length <= maxLength)
            {
                return EncryptData(data, key, bufferLength, suffixData, random);
            }

            // 拆分多个逻辑
            var maxDataLength = maxLength / 2;
            var maxCount = (int) Math.Ceiling(data.Length / (double) maxDataLength);
            var byteList = new byte[maxCount * bufferLength];

            for (var i = 0; i < maxCount; i++)
            {
                var subData = SubByteList(data, i * maxDataLength,
                    Math.Min(maxDataLength, data.Length - i * maxDataLength));

                var dataList = EncryptData(subData, key, bufferLength, suffixData, random);

                Debug.Assert(dataList.Length == bufferLength);
                Buffer.BlockCopy(dataList, 0, byteList, bufferLength * i, bufferLength);
            }

            return byteList;
        }

        /// <summary>
        ///     提供底层的加密算法进行加密，要求 data 明文的长度加上后缀的长度一定小于 <paramref name="bufferLength" /> 的长度。如果想要拿到比较好的密文，让 <paramref name="data" />
        ///     的长度小于 <paramref name="bufferLength" /> 一半
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="key">密码</param>
        /// <param name="bufferLength">缓存长度，这个长度和加密后返回值长度相等</param>
        /// <param name="suffixData">后缀，默认是 <see cref="DefaultSuffixData" /> 的值 </param>
        /// <param name="random">随机数生成</param>
        /// <returns></returns>
        public static byte[] EncryptData(byte[] data, int[] key, int bufferLength = 1024,
            byte[]? suffixData = null, Random? random = null)
        {
            suffixData ??= DefaultSuffixData;

            if (data.Length + suffixData.Length > bufferLength)
                throw new ArgumentOutOfRangeException("需要加密的字符串的长度加上后缀的长度，需要小于缓存长度");

            var buffer = new byte[bufferLength];
            // 和 buffer 构成哈希，用来决定 buffer 上的位置是否被写入值
            var hashList = new bool[bufferLength];
            // 密码位置
            for (var i = 0; i < data.Length + suffixData.Length; i++)
            {
                var hashData = GetPlace(hashList, key, bufferLength, data, suffixData, i);

                byte dataValue;

                if (i < data.Length)
                {
                    dataValue = data[i];
                }
                else
                {
                    dataValue = suffixData[i - data.Length];
                }

                dataValue = (byte) (dataValue + hashData.KeyValue);
                buffer[hashData.HashValue] = dataValue;
                hashList[hashData.HashValue] = true;
            }

            random ??= new Random();
            for (var i = 0; i < hashList.Length; i++)
            {
                if (!hashList[i])
                {
                    buffer[i] = (byte) random.Next(byte.MinValue, byte.MaxValue);
                    hashList[i] = true;
                }
            }

            return buffer;
        }

        /// <summary>
        /// 解密内容，要求解密时传入的参数，包括 <paramref name="key"/> 和 <paramref name="bufferLength"/> 和 <paramref name="suffixData"/> 和加密时完全相同。如有不相同，解密失败，将返回空
        /// </summary>
        /// <param name="encryptionData"></param>
        /// <param name="key"></param>
        /// <param name="bufferLength"></param>
        /// <param name="suffixData"></param>
        /// <returns></returns>
        public static byte[]? Decrypt(byte[] encryptionData, int[] key, int bufferLength = 1024,
            byte[]? suffixData = null)
        {
            if (encryptionData.Length == bufferLength)
            {
                return DecryptData(encryptionData, key, bufferLength, suffixData);
            }

            var blockCount = (int) Math.Ceiling(encryptionData.Length / (double) bufferLength);
            List<byte> byteList = new();
            for (var i = 0; i < blockCount; i++)
            {
                // 这次是不知道实际最终的大小啦
                byte[] encryptionByteList = new byte[bufferLength];

                Buffer.BlockCopy(encryptionData, bufferLength * i, encryptionByteList, 0,
                    // 理论上是刚刚好整数倍的，如果不是，那么在后续解密也会炸
                    Math.Min(bufferLength, encryptionData.Length - bufferLength * i));

                var decryptionData = DecryptData(encryptionByteList, key, bufferLength, suffixData);
                if (decryptionData is null)
                {
                    // 解密失败了
                    return null;
                }

                byteList.AddRange(decryptionData);
            }

            return byteList.ToArray();
        }

        /// <summary>
        /// 解密内容，要求解密时传入的参数，包括 <paramref name="key"/> 和 <paramref name="bufferLength"/> 和 <paramref name="suffixData"/> 和加密时完全相同。如有不相同，解密失败，将返回空。此方法是底层的解密方法，要求传入的 <paramref name="encryptionData"/> 和 <paramref name="bufferLength"/> 相等，如果  <paramref name="encryptionData"/> 长度比 <paramref name="bufferLength"/> 长，那么将只解密 <paramref name="bufferLength"/> 长度数据
        /// </summary>
        /// <param name="encryptionData"></param>
        /// <param name="key"></param>
        /// <param name="bufferLength"></param>
        /// <param name="suffixData"></param>
        /// <returns></returns>
        public static byte[]? DecryptData(byte[] encryptionData, int[] key, int bufferLength = 1024,
            byte[]? suffixData = null)
        {
            suffixData ??= DefaultSuffixData;

            if (encryptionData.Length < bufferLength - 1)
            {
                // 理论上这是相等的，如果小于的话，那么也就是说输入的加密解析不符合
                return null;
            }

            var data = new byte[bufferLength];

            var hashList = new bool[bufferLength];
            for (var i = 0; i < bufferLength; i++)
            {
                var hashData = GetPlace(hashList, key, bufferLength, data, suffixData, i);

                var dataValue = (byte) (encryptionData[hashData.HashValue] - hashData.KeyValue);
                data[i] = dataValue;
                hashList[hashData.HashValue] = true;
            }

            var index = LastIndexOf(data, suffixData);
            if (index > 0)
            {
                // 证明存在后缀内容，那么也就是解密成功了
                return SubByteList(data, 0, index);
            }

            // 其实找不到后缀，也许是忘记后缀了
            // 解密失败了，没有后缀的内容
            return null;
        }

        /// <summary>
        ///     数据的后缀
        /// </summary>
        /// 这里的内容就是“结束”的 Utf-8 内容
        private static readonly byte[] DefaultSuffixData = { 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F };

        /// <summary>
        ///     根据传入参数，获取第 <paramref name="i" /> 个坐标
        /// </summary>
        /// <param name="hashList"></param>
        /// <param name="key">密码</param>
        /// <param name="bufferLength">缓存长度</param>
        /// <param name="data">明文数据</param>
        /// <param name="suffixData">后缀</param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static HashData GetPlace(bool[] hashList, int[] key, int bufferLength, byte[] data, byte[] suffixData,
            int i)
        {
            var keyPlace = i % key.Length;

            // 读取的密码位置，可以计算出字符位置
            var keyValue = key[keyPlace];

            var hashValue = keyValue % bufferLength;
            if (!hashList[hashValue])
            {
                return new HashData(hashValue, keyValue);
            }

            // 第一圈是使用密码自身
            foreach (var _ in key)
            {
                keyPlace++;
                keyPlace = keyPlace % key.Length;
                keyValue += key[keyPlace] * i;
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
            if (i > sizeOfInt)
            {
                // 为什么 j = i - 1 原因是如果是解密的过程，那么当前的明文依然未知
                // 只能取已经解密过的明文来参加计算
                for (var j = i - 1; j >= sizeOfInt; j -= sizeOfInt)
                {
                    var byte1 = GetByte(data, suffixData, j);
                    var byte2 = GetByte(data, suffixData, j - 1);
                    var byte3 = GetByte(data, suffixData, j - 2);
                    var byte4 = GetByte(data, suffixData, j - 3);

                    var n = (byte1 << (3 * 8))
                            + (byte2 << (2 * 8))
                            + (byte3 << (1 * 8))
                            + byte4;
                    n *= i;
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
            else if (i > 0)
            {
                // 为什么 j = i - 1 原因是如果是解密的过程，那么当前的明文依然未知
                for (var j = i - 1; j >= 0; j--)
                {
                    keyValue += GetByte(data, suffixData, j);
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

            static byte GetByte(byte[] a, byte[] b, int n)
            {
                if (n < a.Length)
                {
                    return a[n];
                }

                var length = n - a.Length;
                return b.Length > length ? b[length] : (byte) 0;
            }
        }

        internal static byte[] SubByteList(byte[] data, int startOffset, int length)
        {
            var subData = new byte[length];
            Buffer.BlockCopy(data, startOffset, subData, 0, length);
            return subData;
        }

        internal static int LastIndexOf(byte[] src, byte[] pattern)
        {
            for (var i = src.Length - 1; i >= 0; i--)
            {
                var startOffset = i - (pattern.Length - 1);
                if (startOffset < 0) return -1;

                if (Equals(src, startOffset, pattern))
                {
                    return startOffset;
                }
            }

            return -1;
        }

        private static bool Equals(byte[] src, int startOffset, byte[] pattern)
        {
            for (var i = 0; i < pattern.Length; i++)
            {
                if (src[i + startOffset] != pattern[i])
                {
                    return false;
                }
            }

            return true;
        }

        private readonly struct HashData
        {
            public HashData(int hashValue, int keyValue) : this()
            {
                HashValue = hashValue;
                KeyValue = keyValue;
            }

            public int HashValue { get; }
            public int KeyValue { get; }

            public void Deconstruct(out int hashValue, out int keyValue)
            {
                hashValue = HashValue;
                keyValue = KeyValue;
            }
        }
    }
}
