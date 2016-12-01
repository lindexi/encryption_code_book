// lindexi
// 22:13

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace encryption_code_book.Model
{
    public class NewKeyUrl
    {


        /// <summary>
        ///     输入域名生成密码
        /// </summary>
        /// <param name="name">账户</param>
        /// <param name="key">密码</param>
        /// <param name="url">域名</param>
        /// <param name="num">密码位数</param>
        /// <param name="capitalKey">需要大写字符</param>
        /// <param name="smallKey">需要小写字符</param>
        /// <param name="numKey">需要数字</param>
        /// <param name="specialCharacter">需要特殊字符</param>
        public static string NewKey(string name, string key, string url,
            int num,
            bool capitalKey, bool smallKey, bool numKey, bool specialCharacter, int standby = 0)
        {
            //string url = "https://www.divcss5.com/rumen/r93.shtml";
            try
            {
                Uri uri = new Uri(url);
                url = uri.Host;
            }
            catch
            {

            }
            //System.Console.WriteLine(url); 

            string x = Md5(name + url + standby);
            string x1 = Md5(key + url + standby);
            string x2 = Md5(name + key + url + standby);

            List<KeyAccount> keyAccount = new List<KeyAccount>();

            int n = Keynum(num, capitalKey, smallKey, numKey, specialCharacter, keyAccount);

            int l = num - n;

            string s = KeyCapitals(x, x1, x2, keyAccount);

            key = s.Substring(0, l);

            key = SuppleCapital(key, s, keyAccount);

            n = num - key.Length;

            key += s.Substring(key.Length, n);

            return HaKey(key, x, x1, x2);
        }

        private static string HaKey(string key, string x, string x1, string x2)
        {
            string str = x + x1 + x2;
            int length = 1024;
            bool[] finish = new bool[length];
            char[] temp = new char[length];
            for (int i = 0; i < length; i++)
            {
                temp[i] = '\0';
            }

            for (int i = 0; i < key.Length; i++)
            {
                int n = HaKey(x, x1, x2) ^ str[i % str.Length];
                if (n < 0)
                {
                    n = n * -1;
                }

                n = n % length;
                while (finish[n])
                {
                    n++;
                    if (n >= length)
                    {
                        n = 0;
                    }
                }

                temp[n] = key[i];
            }

            key = "";

            for (int i = 0; i < length; i++)
            {
                if (temp[i] != '\0')
                {
                    key += temp[i];
                }
            }

            return key;
        }

        private static string SuppleCapital(string key, string str, List<KeyAccount> keyAccount)
        {
            bool capital;
            foreach (var temp in keyAccount)
            {
                capital = false;
                for (int i = 0; i < temp.Key.Count; i++)
                {
                    if (key.IndexOf(temp.Key[i]) >= 0)
                    {
                        capital = true;
                        break;
                    }
                }

                if (!capital)
                {
                    int length = key.Length;
                    key += SuppleCapital(length, str, temp.Key);
                }
            }

            return key;
        }

        private static string SuppleCapital(int length, string str, List<string> key)
        {
            for (int i = length; i < str.Length; i++)
            {
                if (key.Any(temp => str[i].ToString() == temp))
                {
                    return str[i].ToString();
                }
            }
            return "";
        }

        private static string KeyCapitals(string x, string x1, string x2, List<KeyAccount> keyAccount)
        {
            double num = 0;
            foreach (var temp in keyAccount)
            {
                num += temp.Scale;
            }

            foreach (var temp in keyAccount)
            {
                temp.Scale = temp.Scale / num * 36;
            }

            num = 0;
            foreach (var temp in keyAccount)
            {
                double n = temp.Scale;
                temp.Scale = temp.Scale + num;
                num += n;
            }


            StringBuilder key = new StringBuilder();
            for (int i = 0; i < x.Length - 1; i++)
            {
                key.Append(Parskeyx(x[i], x[i + 1], keyAccount));
                key.Append(Parskeyx(x1[i], x1[i + 1], keyAccount));
                key.Append(Parskeyx(x2[i], x2[i + 1], keyAccount));
            }

            num = HaKey(x, x1, x2);

            foreach (var temp in keyAccount)
            {
                key.Append(temp.Key[(int)num % temp.Key.Count]);
            }

            return key.ToString();
        }

        private static int HaKey(string x, string x1, string x2)
        {
            string str = x + x1 + x2;
            int n = 0;
            for (int i = 0; i < str.Length; i++)
            {
                n += str[i];
            }
            return n;
        }

        private static string Parskeyx(char x, char x1, List<KeyAccount> keyCapitals)
        {
            int n = 0;
            if ((x <= '9') && (x >= '0'))
            {
                n = x - '0';
            }
            else if ((x <= 'z') && (x >= 'a'))
            {
                n = x - 'a' + 10;
            }
            else if ((x <= 'Z') && (x >= 'A'))
            {
                n = x - 'A' + 10;
            }

            foreach (var temp in keyCapitals)
            {
                if (n - temp.Scale <= 0)
                {
                    n = x1 % temp.Key.Count;
                    return temp.Key[n];
                }
            }
            return null;
        }

        private static int Keynum(int num, bool capitalKey,
            bool smallKey, bool numKey, bool specialCharacter, List<KeyAccount> keyCapital)
        {
            int n = 0;
            if (capitalKey)
            {
                var temp = new KeyAccount()
                {
                    Scale = (double)1,
                    Key = new List<string>()
                };
                keyCapital.Add(temp);
                for (int i = 'A'; i < 'Z' + 1; i++)
                {
                    temp.Key.Add(((char)i).ToString());
                }
                n++;
            }
            if (smallKey)
            {
                var temp = new KeyAccount()
                {
                    Scale = (double)1,
                    Key = new List<string>()
                };
                keyCapital.Add(temp);
                for (int i = 'a'; i < 'z' + 1; i++)
                {
                    temp.Key.Add(((char)i).ToString());
                }
                n++;
            }
            if (numKey)
            {
                keyCapital.Add(new KeyAccount()
                {
                    Scale = (double)1 / 2,
                    Key = new List<string>()
                    {
                        "1",
                        "2",
                        "3",
                        "4",
                        "5",
                        "6",
                        "7",
                        "8",
                        "9",
                        "10"
                    }
                });
                n++;
            }
            if (specialCharacter)
            {
                keyCapital.Add(new KeyAccount()
                {
                    Scale = (double)1 / 2,
                    Key = new List<string>()
                    {
                        "!",
                        "@",
                        "#",
                        "$",
                        "%",
                        "^",
                        "&",
                        "*"
                    }
                });
                n++;
            }

            if ((n == 0) || (num < n))
            {
                throw new ArgumentException();
            }

            return n;
        }

        private static string Md5(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException();
            }

            HashAlgorithmProvider hashAlgorithm =
                HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);

            CryptographicHash cryptographic = hashAlgorithm.CreateHash();

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);

            cryptographic.Append(buffer);

            return CryptographicBuffer.EncodeToHexString(cryptographic.GetValueAndReset());
        }


        private class KeyAccount
        {
            public KeyAccount()
            {
            }

            public double Scale
            {
                set;
                get;
            }

            public List<string> Key
            {
                set;
                get;
            }
        }
    }
}