using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Lindexi.Src.EncryptionAlgorithm;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace EncryptionAlgorithm.Tests;

[TestClass]
public class BinaryEncryptionStreamTest
{
    [ContractTestCase]
    public void TestStream()
    {
        "测试对简单的 Stream 进行加密解密，可以加密解密成功".Test(() =>
        {
            var input = new MemoryStream();
            var output = new MemoryStream();

            var key = new int[]
            {
                '林',
                '德',
                '熙'
            };

            var text = "Hello word, 中文内容输入测试";

            using (var streamWriter = new StreamWriter(input, leaveOpen: true))
            {
                streamWriter.Write(text);
            }

            input.Position = 0;
            BinaryEncryption.EncryptStream(input, output, key);

            input.Position = 0;
            output.Position = 0;
            // 将输出作为输入，测试解密
            var success = BinaryEncryption.TryDecryptStream(output, input, key);
            Assert.IsTrue(success);

            input.Position = 0;

            using (var streamReader = new StreamReader(input))
            {
                var decryptionText = streamReader.ReadToEnd();
                Assert.AreEqual(text, decryptionText);
            }
        });
    }
}
