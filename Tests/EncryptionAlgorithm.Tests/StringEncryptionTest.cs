using System;
using Lindexi.Src.EncryptionAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace EncryptionAlgorithm.Tests
{
    [TestClass]
    public class StringEncryptionTest
    {
        [ContractTestCase]
        public void EncryptText()
        {
            "加密两千个中文字符，可以在解密之后拿到加密前的字符".Test(() =>
            {
                var originCharList = new char[2000];
                const string key = "林德熙";
                for (int i = 0; i < originCharList.Length; i++)
                {
                    originCharList[i] = '林';
                }

                var text = new string(originCharList);

                var byteList = StringEncryption.EncryptText(text, key);

                var decryptionText = StringEncryption.DecryptText(byteList, key);

                Assert.AreEqual(text, decryptionText);
            });

            "加密一百个字符，可以在解密之后拿到加密前的字符".Test(() =>
            {
                var originCharList = new char[100];
                const string key = "林德熙";
                for (int i = 0; i < originCharList.Length; i++)
                {
                    originCharList[i] = 'a';
                }

                var text = new string(originCharList);

                var byteList = StringEncryption.EncryptText(text, key);

                var decryptionText = StringEncryption.DecryptText(byteList, key);

                Assert.AreEqual(text, decryptionText);
            });

            "加密两千个字符，可以在解密之后拿到加密前的字符".Test(() =>
            {
                var originCharList = new char[2000];
                const string key = "林德熙";
                for (int i = 0; i < originCharList.Length; i++)
                {
                    originCharList[i] = 'a';
                }

                var text = new string(originCharList);

                var byteList = StringEncryption.EncryptText(text, key);

                var decryptionText = StringEncryption.DecryptText(byteList, key);

                Assert.AreEqual(text, decryptionText);
            });
        }

        [ContractTestCase]
        public void Encryption()
        {
            "加密简单字符串，解密后能获取加密前的字符串".Test(() =>
            {
                var text = "加密字符串";
                var key = "林德熙";

                var encryptionCharList = StringEncryption.Encrypt(text, key);
                var encryptionString = new string(encryptionCharList);
                var decryptionText = StringEncryption.Decrypt(encryptionString, key);

                Assert.AreEqual(text, decryptionText);
            });
        }

        [ContractTestCase]
        public void CharListToByteList()
        {
            "给定一个中文Char数组，可以转换为Byte数组".Test(() =>
            {
                var text = "林德熙";

                var charList = text.ToCharArray();
                // 能转换成功就是对了
                var byteList = StringEncryption.CharListToByteList(charList);
                var newByteList = StringEncryption.ByteListToCharList(byteList);

                Assert.AreEqual(text, new string(newByteList));
            });
        }

        [ContractTestCase]
        public void ByteListToCharList()
        {
            "给定一个英文字符串，可以转换为Byte数组，可以再转换回原来字符串".Test(() =>
            {
                var text = "aasdfasdfasdf";

                var charList = text.ToCharArray();
                var byteList = StringEncryption.CharListToByteList(charList);
                var newByteList = StringEncryption.ByteListToCharList(byteList);

                Assert.AreEqual(text, new string(newByteList));
            });
        }
    }
}
