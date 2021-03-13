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
        public void Encryption()
        {
            "加密简单字符串，解密后能获取加密前的字符串".Test(() =>
            {
                var text = "加密字符串";
                var key = "林德熙";

                var encryptionCharList = StringEncryption.Encrypt(text,key);
                var encryptionString = new string(encryptionCharList);
                var decryptionText = StringEncryption.Decrypt(encryptionString,key);

                Assert.AreEqual(text, decryptionText);
            });
        }
    }
}
