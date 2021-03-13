using System;
using System.Text;

namespace Lindexi.Src.EncryptionAlgorithm
{
    public static class StringEncryption
    {
        // 要求 text 小于 1024 个字符
        public static char[] Encrypt(string text, string key)
        {
            var random = new Random();

            char[] tempCharList = new char[TempStringLength];
            var str = text;
            str += Suffix;
            for (var i = 0; i < TempStringLength; i++)
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
                hashValue = hashValue % TempStringLength;
                while (tempCharList[hashValue] != Convert.ToChar(0)) //如果位置有别的字符就下一个，到没有字符位置
                {
                    hashValue++;
                    if (hashValue >= TempStringLength)
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
            for (var i = 0; i < TempStringLength; i++)
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


        public static string Decrypt(string str, string key)
        {
            StringBuilder temp = new StringBuilder();
            char[] encryptionCharList = str.ToCharArray();

            // 是否完全完成
            var isAccomplish = false;
            // keyPlace 密码位置
            var keyPlace = 0;
            if (encryptionCharList.Length < TempStringLength - 1)
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
                hashValue = hashValue % TempStringLength; //密码给字符所在位置
                while (encryptionCharList[hashValue] == Convert.ToChar(0))
                {
                    // 其实这里是不会进入的，原因是加密有填补空白
                    hashValue++;
                    if (hashValue >= TempStringLength)
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
                if (temp.Length == TempStringLength)
                {
                    isAccomplish = true;
                }
            }

            string tempText = temp.ToString();
            int tempIndex = tempText.LastIndexOf(Suffix, StringComparison.Ordinal);
            if (tempIndex > 0)
            {
                // 证明存在后缀内容，那么也就是解密成功了
                return tempText.Substring(0, tempIndex);
            }

            // 解密失败了，没有后缀的内容
            return null;
        }

        /// <summary>
        /// 缓存长度
        /// </summary>
        private const int TempStringLength = 1024;

        /// <summary>
        /// 后缀
        /// </summary>
        private const string Suffix = "结束";
    }
}