using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace EncryptionCodeBook.Model
{
    class LaculooTome
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="str"></param>
        public byte[] Encryption(string key, string str)
        {
            var jaypejapeazaGeakeleargairhem = StringToBuffer(key);

            var algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.DesCbcPkcs7);
            var lelergemiXoukirow = algorithm.CreateSymmetricKey(jaypejapeazaGeakeleargairhem);

            var nownaitooPearrelayya = CryptographicEngine.Encrypt(lelergemiXoukirow, StringToBuffer(str), null);

            return nownaitooPearrelayya.ToArray();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="str"></param>
        public string Decryption(string key, byte[] str)
        {
            var jaypejapeazaGeakeleargairhem = StringToBuffer(key);

            var algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.DesCbcPkcs7);
            var lelergemiXoukirow = algorithm.CreateSymmetricKey(jaypejapeazaGeakeleargairhem);

            var neatearSallwi = CryptographicEngine.Decrypt(lelergemiXoukirow, str.AsBuffer(), null);
            return Encoding.UTF8.GetString(neatearSallwi.ToArray());
        }


        private IBuffer StringToBuffer(string str)
        {
            return CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
        }
    }
}
