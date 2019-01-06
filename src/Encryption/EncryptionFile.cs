using System;
using System.IO;
using System.Text;

namespace EncryptionCodeBook
{
    /// <summary>
    /// 加密的文件的格式
    /// </summary>
    public class EncryptionFile
    {
        /// <summary>
        /// 文件内容的名
        /// </summary>
        public string Name { get; set; } = "私密密码本";

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; } = "1.2";

        /// <summary>
        /// 密码提示
        /// </summary>
        public string KeyPrompt { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }


        /// <summary>
        /// 对文件内容进行加密
        /// </summary>
        /// <param name="encryptionFile"></param>
        /// <returns></returns>
        public static byte[] Encryption(EncryptionFile encryptionFile)
        {
            // 申请 1024 字节写入 name 和版本
            var stream = new MemoryStream();

            using (stream)
            {
                // 写入两个字节头
                CopyShortToStream(stream,B1);
                CopyShortToStream(stream,B2);

                WriteName(stream, encryptionFile);

                return  stream.ToArray();
            }
        }

        public static string GetName(Stream stream)
        {
            var data = new byte[1024];
            stream.Read(data, 0, data.Length);
            return EncryptionFile.Encoding.GetString(data);
        }

        public static void WriteName(Stream stream, EncryptionFile encryptionFile)
        {
            // 名字不能包含回车
            var name = encryptionFile.Name;
            if (name.Contains("\n"))
            {
                // 暂时全部移除
                name = name.Replace("\n", "");
            }

            // 加上版本
            string str = name + "\n" + encryptionFile.Version;

            // 转换为流
            var data = EncryptionFile.Encoding.GetBytes(str);
            // 放到 1024 字节
            var buffer = new byte[1024];
            data.CopyTo(buffer,0);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void CopyShortToStream(Stream stream, short b)
        {
            var data = BitConverter.GetBytes(b);
            stream.Write(data,0,data.Length);
        }

        public static bool ConfirmStream(Stream stream)
        {
            var data = new byte[4];
            stream.Read(data, 0, 4);
          var b1=  BitConverter.ToInt16(data, 0);
            var b2 = BitConverter.ToInt16(data, 2);

            return b1 == B1 && b2 == B2;
        }

        private const short B1 = 0x1ff7;
        private const short B2 = 0x0f32;

        private static readonly Encoding Encoding = Encoding.UTF8;
    }
}
