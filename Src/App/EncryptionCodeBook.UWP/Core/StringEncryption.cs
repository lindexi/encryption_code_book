using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.Devices.SmartCards;

namespace encryption_code_book.Model
{
    public class StringEncryption
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] Encryption(string str, string key)
        {
            return Lindexi.Src.EncryptionAlgorithm.StringEncryption.EncryptText(str, key);
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <returns></returns>
        public string Decryption(byte[] encryptionData, string key)
        {
            return Lindexi.Src.EncryptionAlgorithm.StringEncryption.DecryptText(encryptionData, key);

        }
        /// <summary>
        /// 确认密码
        /// </summary>
        /// <param name="keystr">密码加密</param>
        /// <param name="key">要确认密码</param>
        /// <returns></returns>
        public bool Confirm(string keystr, string key)
        {
            List<byte> byteList = new List<byte>();
            for (var i = 0; i < keystr.Length; i++)
            {
                var c = keystr[i];
                byteList.AddRange(BitConverter.GetBytes(c));
            }
            return string.Equals(Decryption(byteList.ToArray(), key), Nmd5(key));
        }

        /// <summary>
        /// 确认密码
        /// </summary>
        /// <param name="confirmKeyByteList">密码加密</param>
        /// <param name="key">要确认密码</param>
        /// <returns></returns>
        public bool Confirm(byte[] confirmKeyByteList, string key)
        {
            return string.Equals(Decryption(confirmKeyByteList, key), Nmd5(key));
        }

        /// <summary>
        /// 加密多次的 Md5 值
        /// </summary>
        /// <param name="key">密码</param>
        /// <returns>加密后密码</returns>
        public string Nmd5(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return "".PadRight(32, '0');
            }
            else
            {
                return GetMD5(key);
            }
        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="str">要加密字符串</param>
        /// <returns>加密后密码</returns>
        private static string GetMD5(string str)
        {
            Windows.Security.Cryptography.Core.HashAlgorithmProvider objAlgProv = Windows.Security.Cryptography.Core.HashAlgorithmProvider.OpenAlgorithm(Windows.Security.Cryptography.Core.HashAlgorithmNames.Md5);
            Windows.Security.Cryptography.Core.CryptographicHash md5 = objAlgProv.CreateHash();
             Windows.Storage.Streams.IBuffer bufferMessage = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(str, Windows.Security.Cryptography.BinaryStringEncoding.Utf16BE);
            md5.Append(bufferMessage );
            Windows.Storage.Streams.IBuffer bufferHash = md5.GetValueAndReset();
            return Windows.Security.Cryptography.CryptographicBuffer.EncodeToBase64String(bufferHash);
        }
    }

}
