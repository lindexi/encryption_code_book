using System;
using System.Diagnostics;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace encryption_code_book
{
    public class string_decryption
    {
        ///需要 using System.IO;    
        private string_decryption()
        {
            file_address = @".\data.data";
            _temp_string_lenth = 1024;
        }
        public string_decryption(string file_加密的文件 , int key_length_more512)
        {
            key_length_more512 = 1024;
            _temp_string_lenth = key_length_more512;

            file_address = file_加密的文件;
            //file = new FileStream(f_文件地址 , FileMode.OpenOrCreate);
        }
        public string_decryption(string key)
        {
            int 密钥_大于512;
            密钥_大于512 = 1024;
            _temp_string_lenth = 密钥_大于512;
            this.key = key;
        }

        public static string_decryption g_获得类()
        {
            return _string_decryption;
        }
        public static string_decryption g_获得类(string file_加密的文件)
        {
            _string_decryption = new string_decryption(file_加密的文件 , 1024);
            return _string_decryption;
        }

        ~string_decryption()
        {


        }

        public string key
        {
            set
            {
                _key = value;
                h_后缀 = n_md5(_key)+ "结束";//密码改，后缀n_md5
            }
            get
            {
                if (_key.Length <= 0)
                {
                    return "林德熙";
                }
                return _key;
            }            
        }
        /// <summary>
        /// 加密文件绝对位置
        /// </summary>
        public string file_address
        {
            set
            {
                _file_address = value;
            }
            get
            {
                return _file_address;
            }
        }
        public string h_后缀
        {
            set
            {
                _后缀 = value;
            }
            get
            {
                if (_后缀 == null || _后缀.Length==0)
                {
                    return "结束";
                }
                else
                {
                    return _后缀;
                }
            }
        }

        public string encryption(string str)
        {
            char[] temp_str = new char[_temp_string_lenth];
            int i , has , key_place;//has字符位置，key_place密码位置
            //str = encryptDes(str);
            str += h_后缀;
            str.PadRight(_temp_string_lenth);
            for (i = 0; i < _temp_string_lenth; i++)
            {
                temp_str[i] = Convert.ToChar(0);
            }
            key_place = 0;
            for (i = 0; i < str.Length; i++)
            {
                has = Convert.ToInt32(key[key_place]);
                has = has % _temp_string_lenth;
                while (temp_str[has] != Convert.ToChar(0))//如果位置有别的字符就下一个，到没有字符位置
                {
                    has++;
                    if (has >= _temp_string_lenth)
                    {
                        has = 0;//出错，_temp_string_lenth太小
                    }
                    //has=has>=_temp_string_lenth?0:has++;
                }
                //temp_str[l] = (char)(str[i]);//+key[key_l]);
                temp_str[has] = (char)((str[i]) + key[key_place]%1024);
                key_place++;
                if (key_place == key.Length)
                {
                    key_place = 0;
                }
                //key_place=key_place>=key.length?0:key_place++;
            }
            //把空填充
            for (i = 0; i < _temp_string_lenth; i++)
            {
                if (temp_str[i] == Convert.ToChar(0))
                {
                    temp_str[i] = Convert.ToChar(ran); //% 1000+1);
                }
            }
            return new string(temp_str);
            //string s = new string(temp_str);
            //return s;
            //return null;
        }
        public string decryption(string str)
        {
            StringBuilder temp = new StringBuilder();
            char[] jie = str.ToCharArray();
            int has , key_place;//has字符位置，key_place密码位置
            //int accomplish_position;//accomplish_position==h_后缀.length accomplish=true
            bool accomplish;
            accomplish = false;//初始
            has = 0;
            key_place = 0;
            //accomplish_position = 0;
            if (jie.Length < _temp_string_lenth - 1)
            {
                Debug.Write("错" + jie.Length.ToString());
                return null;
            }
            while (accomplish == false)//我while(true)
            {
                if (accomplish)
                {
                    break;
                }
                has = Convert.ToInt32(key[key_place]);
                has = has % _temp_string_lenth;//密码给字符所在位置
                while (jie[has] == Convert.ToChar(0))
                {
                    has++;
                    if (has >= _temp_string_lenth)
                    {
                        //accomplish = true;
                        has = 0;
                        //break;
                    }
                }
                
                //s.Append((char)(jie[l]));//-key[key_l]));
                temp.Append((char)((jie[has]) - key[key_place]%1024));
                jie[has] = Convert.ToChar(0);//把原来位置0
                key_place++;
                if (key_place == key.Length)
                {
                    key_place = 0;
                }
                if(temp.Length==_temp_string_lenth)
                {
                    accomplish = true;
                }
                //if (temp[temp.Length - 1] == h_后缀[accomplish_position])
                //{
                //    accomplish_position++;
                //    if (accomplish_position == h_后缀.Length)
                //    {
                //        accomplish = true;
                //    }
                //}
                //else
                //{
                //    accomplish_position = 0;
                //}
            }
            string temp_str = temp.ToString();
            //int temp_l = temp_str.LastIndexOf("结束");
            int temp_l = temp_str.LastIndexOf(h_后缀);
            if (temp_l > 0)
            {
                //return decryptDes(temp_str.Substring(0 , temp_l));
                return temp_str.Substring(0 , temp_l);
            }
            if(temp_l==0)
            {
                return temp_str;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 加密key[0].toint次md5
        /// </summary>
        /// <param name="key">密码</param>
        /// <returns>加密后密码</returns>
        public string n_md5(string key)
        {
            string temp;
            //int i;
            //int str_0_length;
            if (string.IsNullOrEmpty(key))
            {
                temp = "";
                return temp.PadRight(32 , '0');
            }
            else
            {
                return get_MD5(key);
            }
            //str_0_length = Convert.ToInt32(key[0]);
            //temp = get_MD5(key);
            //for (i = 1; i < str_0_length; i++)
            //{
            //    temp = get_MD5(temp);
            //}

            //return temp;
        }
        /// <summary>
        /// 缓冲区长度
        /// </summary>
        private int _temp_string_lenth;
        private static string_decryption _string_decryption = new string_decryption();
        private Random _random = new Random();
        //加密文件的路径
        private string _file_address;
        private string _key;
        private string _后缀;
        
        private int ran
        {
            set
            {
                _random = new Random(value);
            }
            get
            {
                return _random.Next(2) == 0 ? _random.Next(19968 , 40864) : _random.Next(33 , 126);
            }
        }
        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="str">要加密字符串</param>
        /// <returns>加密后密码</returns>
        private string get_MD5(string str)
        {
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash md5 = objAlgProv.CreateHash();
            IBuffer buffMsg1 = CryptographicBuffer.ConvertStringToBinary(str , BinaryStringEncoding.Utf16BE);
            md5.Append(buffMsg1);
            IBuffer buffHash1 = md5.GetValueAndReset();
            return CryptographicBuffer.EncodeToBase64String(buffHash1);
            //return str;
            //System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //byte[] temp;
            //StringBuilder strb = new StringBuilder();
            //temp = md5.ComputeHash(Encoding.Unicode.GetBytes(str));
            //md5.Clear();
            //for (int i = 0; i < temp.Length; i++)
            //{
            //    strb.Append(temp[i].ToString("X").PadLeft(2 , '0'));
            //}
            //return strb.ToString().ToLower();
            //return str.GetHashCode().ToString();
            //return sTemp.ToLower();
        }
    }
}
