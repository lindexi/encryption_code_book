using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.Extensions.Contracts;

namespace TirdairwowNurma
{
    [TestClass]
    public class EncryptionFile
    {
        [ContractTestCase]
        public void Encryption()
        {
            "测试读取name方法".Test(() =>
            {
                var stream = new MemoryStream();
                using (stream)
                {
                    var encryptionFile = new EncryptionCodeBook.EncryptionFile();
                    EncryptionCodeBook.EncryptionFile.WriteName(stream, encryptionFile);

                    stream.Seek(0, SeekOrigin.Begin);

                    var name = EncryptionCodeBook.EncryptionFile.GetName(stream);

                    // 拆开
                    var gairmowGajar = name.Split("\n");
                    Assert.AreEqual(2, gairmowGajar.Length);
                    name = gairmowGajar[0];
                    var version = gairmowGajar[1].Trim('\0');
                    Assert.AreEqual(true, encryptionFile.Name.Equals(name));
                    Assert.AreEqual(true, encryptionFile.Version.Equals(version));
                }
            });

            "测试写入name方法".Test(() =>
            {
                var stream = new MemoryStream();
                using (stream)
                {
                    var encryptionFile = new EncryptionCodeBook.EncryptionFile();
                    EncryptionCodeBook.EncryptionFile.WriteName(stream, encryptionFile);

                    Assert.AreEqual(1024, stream.Position);
                }
            });

            "测试从short复制到stream方法".Test(() =>
            {
                var stream = new MemoryStream();
                using (stream)
                {
                    EncryptionCodeBook.EncryptionFile.CopyShortToStream(stream, 0x1ff7);
                    EncryptionCodeBook.EncryptionFile.CopyShortToStream(stream, 0x0f32);

                    stream.Seek(0, SeekOrigin.Begin);

                    Assert.AreEqual(true, EncryptionCodeBook.EncryptionFile.ConfirmStream(stream));
                }
            });

            "测试文件开始".Test(() =>
            {
                byte b1 = unchecked((byte) 0x1ff7);

                Assert.AreEqual(b1, unchecked((byte) 0x1ff7));
            });
        }
    }
}