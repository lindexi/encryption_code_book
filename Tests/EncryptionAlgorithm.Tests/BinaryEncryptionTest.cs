using System.Text;
using Lindexi.Src.EncryptionAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace EncryptionAlgorithm.Tests
{
    [TestClass]
    public class BinaryEncryptionTest
    {
        [ContractTestCase]
        public void EncryptText()
        {
            "加密两千个中文字符，可以在解密之后拿到加密前的字符".Test(() =>
            {
                var originCharList = new char[2000];
                int[] key = new int[]
                {
                    '林',
                    '德',
                    '熙'
                };
                for (int i = 0; i < originCharList.Length; i++)
                {
                    originCharList[i] = '林';
                }

                var text = new string(originCharList);
                var textData = Encoding.UTF8.GetBytes(text);

                var byteList = BinaryEncryption.Encrypt(textData, key);

                var decryptionData = BinaryEncryption.Decrypt(byteList, key);
                Assert.IsNotNull(decryptionData);

                var decryptionText = Encoding.UTF8.GetString(decryptionData);

                Assert.AreEqual(text, decryptionText);
            });

            "加密一百个字符，可以在解密之后拿到加密前的字符".Test(() =>
            {
                var originCharList = new char[100];
                int[] key = new int[]
                {
                    '林',
                    '德',
                    '熙'
                };
                for (int i = 0; i < originCharList.Length; i++)
                {
                    originCharList[i] = 'a';
                }

                var text = new string(originCharList);
                var textData = Encoding.UTF8.GetBytes(text);

                var byteList = BinaryEncryption.Encrypt(textData, key);

                var decryptionData = BinaryEncryption.Decrypt(byteList, key);
                Assert.IsNotNull(decryptionData);

                var decryptionText = Encoding.UTF8.GetString(decryptionData);

                Assert.AreEqual(text, decryptionText);
            });

            "加密两千个字符，可以在解密之后拿到加密前的字符".Test(() =>
            {
                var originCharList = new char[2000];
                int[] key = new int[]
                {
                    '林',
                    '德',
                    '熙'
                };
                for (int i = 0; i < originCharList.Length; i++)
                {
                    originCharList[i] = 'a';
                }

                var text = new string(originCharList);
                var textData = Encoding.UTF8.GetBytes(text);

                var byteList = BinaryEncryption.Encrypt(textData, key);

                var decryptionData = BinaryEncryption.Decrypt(byteList, key);
                Assert.IsNotNull(decryptionData);

                var decryptionText = Encoding.UTF8.GetString(decryptionData);

                Assert.AreEqual(text, decryptionText);
            });
        }


        [ContractTestCase]
        public void LastIndexOf()
        {
            "传入一个完全没有匹配数据的数组，可以返回没有找到".Test(() =>
            {
                var data = new byte[100];
                var pattern = new byte[] { 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F };

                var value = BinaryEncryption.LastIndexOf(data, pattern);
                Assert.AreEqual(-1, value);
            });

            "传入一个不在最后存在匹配的数组，可以返回找到匹配".Test(() =>
            {
                var data = new byte[] { 0x00, 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F, 0x00 };
                var pattern = new byte[] { 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F };

                var value = BinaryEncryption.LastIndexOf(data, pattern);
                Assert.AreEqual(1, value);
            });

            "一个二进制数组存在两个数据完全匹配，可以返回最后一个匹配的数据".Test(() =>
            {
                var data = new byte[] { 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F, 0xE7, 0xBB, 0x93, 0xE6, 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F };
                var pattern = new byte[] { 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F };

                var value = BinaryEncryption.LastIndexOf(data, pattern);
                Assert.AreEqual(10, value);
            });

            "求两个完全相等的二进制数组是否相等，可以返回相等".Test(() =>
            {
                var data = new byte[] { 0xE7, 0xBB, 0x93, 0xE6, 0x9D, 0x9F };
                var pattern = data;

                var value = BinaryEncryption.LastIndexOf(data, pattern);
                Assert.AreEqual(0, value);
            });
        }
    }
}
