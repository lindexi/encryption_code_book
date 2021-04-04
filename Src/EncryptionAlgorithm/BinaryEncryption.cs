using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lindexi.Src.EncryptionAlgorithm
{
    public static class BinaryEncryption
    {
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
            else
            {
                // 拆分多个逻辑
                var maxDataLength = maxLength / 2;
                var maxCount = (int)Math.Ceiling(data.Length / (double)maxDataLength);
                var byteList = new byte[maxCount * bufferLength];

                for (int i = 0; i < maxCount; i++)
                {
                    var subData = SubByteList(data, i * maxDataLength,
                        Math.Min(maxDataLength, data.Length - i * maxDataLength));

                    var dataList = EncryptData(subData, key, bufferLength, suffixData, random);

                    Debug.Assert(dataList.Length == bufferLength);
                    Buffer.BlockCopy(dataList, 0, byteList, bufferLength * i, bufferLength);
                }

                return byteList;
            }
        }

        private static byte[] EncryptData(byte[] data, int[] key, int bufferLength = 1024,
            byte[]? suffixData = null, Random? random = null)
        {
            suffixData ??= DefaultSuffixData;

            if (data.Length + suffixData.Length > bufferLength)
            {
                throw new ArgumentOutOfRangeException($"需要加密的字符串的长度加上后缀的长度，需要小于缓存长度");
            }

            var buffer = new byte[bufferLength];
            // 和 buffer 构成哈希，用来决定 buffer 上的位置是否被写入值
            var hashList = new bool[bufferLength];
            // 密码位置
            var keyPlace = 0;
            for (int i = 0; i < data.Length + suffixData.Length; i++)
            {
                // 读取的密码位置，可以计算出字符位置
                var keyValue = key[keyPlace];
                var hashValue = keyValue % bufferLength;
                //如果位置有别的数据就读取下一个，到没有字符位置
                while (hashList[hashValue])
                {
                    hashValue++;
                    if (hashValue >= bufferLength)
                    {
                        hashValue = 0;
                    }
                }

                byte dataValue;

                if (i < data.Length)
                {
                    dataValue = data[i];
                }
                else
                {
                    dataValue = suffixData[i - data.Length];
                }

                dataValue = (byte)(dataValue + keyValue);
                buffer[hashValue] = dataValue;
                hashList[hashValue] = true;

                keyPlace++;
                if (keyPlace == key.Length)
                {
                    keyPlace = 0;
                }
            }

            random ??= new Random();
            for (var i = 0; i < hashList.Length; i++)
            {
                if (!hashList[i])
                {
                    buffer[i] = (byte)random.Next(byte.MinValue, byte.MaxValue);
                    hashList[i] = true;
                }
            }

            return buffer;
        }

        public static byte[]? Decrypt(byte[] encryptionData, int[] key, int bufferLength = 1024,
            byte[]? suffixData = null)
        {
            if (encryptionData.Length == bufferLength)
            {
                return DecryptData(encryptionData, key, bufferLength, suffixData);
            }
            else
            {
                var blockCount = (int)Math.Ceiling(encryptionData.Length / (double)bufferLength);
                List<byte> byteList = new List<byte>();
                for (int i = 0; i < blockCount; i++)
                {
                    // 这次是不知道实际最终的大小啦
                    byte[] encryptionByteList = new byte[bufferLength];

                    Buffer.BlockCopy(encryptionData, bufferLength * i, encryptionByteList, 0,
                        // 理论上是刚刚好整数倍的，如果不是，那么在后续解密也会炸
                        Math.Min(bufferLength, encryptionData.Length - bufferLength * i));

                    var decryptionData = DecryptData(encryptionByteList, key, bufferLength,suffixData);
                    if (decryptionData is null)
                    {
                        // 解密失败了
                        return null;
                    }
                    byteList.AddRange(decryptionData);
                }

                return byteList.ToArray();
            }
        }

        private static byte[]? DecryptData(byte[] encryptionData, int[] key, int bufferLength = 1024,
            byte[]? suffixData = null)
        {
            suffixData ??= DefaultSuffixData;

            // keyPlace 密码位置
            var keyPlace = 0;
            if (encryptionData.Length < bufferLength - 1)
            {
                // 理论上这是相等的，如果小于的话，那么也就是说输入的加密解析不符合
                return null;
            }

            var data = new byte[bufferLength];

            var hashList = new bool[bufferLength];
            for (int i = 0; i < bufferLength; i++)
            {
                // keyPlace 密码位置
                var keyValue = key[keyPlace];
                var hashValue = keyValue % bufferLength;
                while (hashList[hashValue])
                {
                    hashValue++;
                    if (hashValue >= bufferLength)
                    {
                        hashValue = 0;
                    }
                }

                var dataValue = (byte)(encryptionData[hashValue] - keyValue);
                data[i] = dataValue;
                hashList[hashValue] = true;

                keyPlace++;
                if (keyPlace == key.Length)
                {
                    keyPlace = 0;
                }
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
                if (startOffset < 0)
                {
                    return -1;
                }

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


        /// <summary>
        /// 数据的后缀
        /// </summary>
        /// 这里的内容就是“结束”的 Utf-8 内容
        private static readonly byte[] DefaultSuffixData = new byte[] { 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F };
    }
}