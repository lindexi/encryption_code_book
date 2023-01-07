using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lindexi.Src.EncryptionAlgorithm
{
    public static class StringEncryption
    {
        private const int DefaultTempStringLength = 1024;
        private const string DefaultSuffix = "结束";

        public static byte[] EncryptText(string text, string key)
        {
            // 如果超过了 DefaultTempStringLength 的长度，那么自动拆分
            const int maxTextLengthInt = DefaultTempStringLength / 2;
            const double maxTextLength = maxTextLengthInt;

            if (text.Length < maxTextLength)
            {
                var charList = Encrypt(text, key);
                return CharListToByteList(charList);
            }
            else
            {
                // 拆分多个逻辑
                var maxCount = (int) Math.Ceiling(text.Length / maxTextLength);
                const int sizeOfChar = 2; // sizeof char
                var byteList = new byte[maxCount * DefaultTempStringLength * sizeOfChar];

                for (int i = 0; i < maxCount; i++)
                {
                    var subString = text.Substring(i * maxTextLengthInt, Math.Min(maxTextLengthInt, text.Length - i * maxTextLengthInt));
                    var charList = Encrypt(subString, key);

                    const int charListByteLength = DefaultTempStringLength * sizeOfChar;
                    Buffer.BlockCopy(charList, 0, byteList, charListByteLength * i, charListByteLength);
                }

                return byteList;
            }
        }

        internal static byte[] CharListToByteList(char[] charList)
        {
            var byteList = new byte[charList.Length * 2];

            Buffer.BlockCopy(charList, 0, byteList, 0, byteList.Length);
            return byteList;
        }

        internal static char[] ByteListToCharList(byte[] byteList)
        {
            var charList = new char[byteList.Length / 2];

            Buffer.BlockCopy(byteList, 0, charList, 0, byteList.Length);

            return charList;
        }


        // 要求 text 小于 1024 个字符
        internal static char[] Encrypt(string text, string key, int tempStringLength = DefaultTempStringLength, string suffix = DefaultSuffix)
        {
            // 缓存长度
            // 后缀

            if (text.Length + suffix.Length > tempStringLength)
            {
                throw new ArgumentOutOfRangeException($"需要加密的字符串的长度加上后缀的长度，需要小于缓存长度");
            }

            var random = new Random();

            char[] tempCharList = new char[tempStringLength];
            var str = text;
            str += suffix;
            for (var i = 0; i < tempStringLength; i++)
            {
                // 理论上是不存在字符串里面有 0 这个字符的，因此可以在后续判断是否为零，如果是那么证明这里是不存在内容
                // 其实也可以不需要清空，但是为了在 .NET 5 里面，减少因为申请数组的时候，设置是清空的有锅，因此这里依然保持和论文相同的方式，先清空
                tempCharList[i] = Convert.ToChar(0);
            }

            // 密码位置
            var keyPlace = 0;
            for (var i = 0; i < str.Length; i++)
            {
                // keyPlace 密码位置
                var keyChar = key[keyPlace];
                // hashValue 字符位置
                var hashValue = Convert.ToInt32(keyChar);
                hashValue = hashValue % tempStringLength;
                while (tempCharList[hashValue] != Convert.ToChar(0)) //如果位置有别的字符就下一个，到没有字符位置
                {
                    hashValue++;
                    if (hashValue >= tempStringLength)
                    {
                        hashValue = 0; //出错 TempStringLength 太小
                    }
                }

                tempCharList[hashValue] = (char) ((str[i]) + keyChar % 1024);
                keyPlace++;
                if (keyPlace == key.Length)
                {
                    keyPlace = 0;
                }
            }

            // 把空填充
            for (var i = 0; i < tempStringLength; i++)
            {
                if (tempCharList[i] == Convert.ToChar(0))
                {
                    // 加入的内容就是英文和汉子内容
                    var ran = random.Next(2) == 0 ? random.Next(19968, 40864) : random.Next(33, 126);
                    tempCharList[i] = Convert.ToChar(ran);
                }
            }

            return tempCharList;
        }

        public static string? DecryptText(byte[] encryptionData, string key)
        {
            const int sizeOfChar = 2; // sizeof char
            const int encryptionByteListLengthInt = DefaultTempStringLength * sizeOfChar;
            const double encryptionByteListLength = encryptionByteListLengthInt;
            var blockCount = (int) Math.Ceiling(encryptionData.Length / encryptionByteListLength);
            if (blockCount == 1)
            {
                var charList = ByteListToCharList(encryptionData);
                return Decrypt(charList, key);
            }
            else
            {
                StringBuilder temp = new StringBuilder();

                for (int i = 0; i < blockCount; i++)
                {
                    char[] encryptionCharList = new char[DefaultTempStringLength];

                    Buffer.BlockCopy(encryptionData, encryptionByteListLengthInt * i, encryptionCharList, 0,
                        // 理论上是刚刚好整数倍的，如果不是，那么在后续解密也会炸
                        Math.Min(encryptionByteListLengthInt, encryptionData.Length - encryptionByteListLengthInt * i));

                    temp.Append(Decrypt(encryptionCharList, key));
                }

                return temp.ToString();
            }
        }

        internal static string? Decrypt(string str, string key, int tempStringLength = DefaultTempStringLength, string suffix = DefaultSuffix)
        {
            // 缓存长度
            // 后缀
            char[] encryptionCharList = str.ToCharArray();

            return Decrypt(encryptionCharList, key, tempStringLength, suffix);
        }

        internal static string? Decrypt(char[] encryptionCharList, string key, int tempStringLength = DefaultTempStringLength, string suffix = DefaultSuffix)
        {
            StringBuilder temp = new StringBuilder();
            // 是否完全完成
            var isAccomplish = false;
            // keyPlace 密码位置
            var keyPlace = 0;
            if (encryptionCharList.Length < tempStringLength - 1)
            {
                // 理论上这是相等的，如果小于的话，那么也就是说输入的加密解析不符合
                return null;
            }

            while (isAccomplish == false)
            {
                // keyPlace 密码位置
                var keyChar = key[keyPlace];
                // 字符位置
                var hashValue = Convert.ToInt32(keyChar);
                hashValue = hashValue % tempStringLength; //密码给字符所在位置
                while (encryptionCharList[hashValue] == Convert.ToChar(0))
                {
                    hashValue++;
                    if (hashValue >= tempStringLength)
                    {
                        hashValue = 0;
                    }
                }

                temp.Append((char) ((encryptionCharList[hashValue]) - keyChar % 1024));
                encryptionCharList[hashValue] = Convert.ToChar(0); //把原来位置0
                keyPlace++;
                if (keyPlace == key.Length)
                {
                    keyPlace = 0;
                }

                // 完全解密完成了
                if (temp.Length == tempStringLength)
                {
                    isAccomplish = true;
                }
            }

            string tempText = temp.ToString();
            int tempIndex = tempText.LastIndexOf(suffix, StringComparison.Ordinal);
            if (tempIndex > 0)
            {
                // 证明存在后缀内容，那么也就是解密成功了
                return tempText.Substring(0, tempIndex);
            }

            // 其实找不到后缀，也许是忘记后缀了
            // 解密失败了，没有后缀的内容
            return null;
        }
    }
}
