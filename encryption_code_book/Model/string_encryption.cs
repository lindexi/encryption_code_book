using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace encryption_code_book.Model
{
    public class string_encryption
    {
        public string_encryption()
        {
            key = "林德熙";
            _temp_string_lenth = 1024;
        }
        public string_encryption(string key)
        {
            int 密钥_大于512;
            密钥_大于512 = 1024;
            _temp_string_lenth = 密钥_大于512;
            this.key = key;
        }

        public static string_encryption g_获得类()
        {
            if (_string_decryption == null)
            {
                _string_decryption = new string_encryption();
            }
            return _string_decryption;
        }
     

        ~string_encryption()
        {


        }

        public string key
        {
            set
            {
                _key = value;
                h_后缀 = n_md5(_key) + "结束";//密码改，后缀n_md5
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

        public string h_后缀
        {
            set
            {
                _后缀 = value;
            }
            get
            {
                if (_后缀 == null || _后缀.Length == 0)
                {
                    return "结束";
                }
                else
                {
                    return _后缀;
                }
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string encryption(string str , string key)
        {
            //验证密钥
            int temp_length = 0;
            if (!string.IsNullOrEmpty(key))
            {
                temp_length = 1024;//缓冲区
            }
            string hstr = n_md5(key) + "结束";//后缀
            double per = 0.5;//明文密文比
            //分明文            
            List<string> clearstr = new List<string>();//明文
            //均匀分？最小
            int count = (int)( temp_length * per ) - hstr.Length;
            while (str.Length >= temp_length * per - hstr.Length)
            {
                clearstr.Add(str.Substring(0 , count));//+hstr);
                str = str.Substring(count);
            }
            clearstr.Add(str);
            //加密
            StringBuilder ciphertext = new StringBuilder();//密文
            int[] position = new int[temp_length];
            hash(ref position , temp_length , key);
            foreach (string temp in clearstr)
            {
                ciphertext.Append(encryption_string(temp , temp_length , hstr , key , position));
            }
            return ciphertext.ToString();
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string decryption(string str , string key)
        {
            //验证密钥
            int temp_length = 0;
            if (!string.IsNullOrEmpty(key))
            {
                temp_length = 1024;//缓冲区
            }
            string hstr = n_md5(key) + "结束";//后缀
            //分密文
            List<string> ciphertext = new List<string>();//密文
            while (str.Length > temp_length)
            {
                ciphertext.Add(str.Substring(0 , temp_length));
                str = str.Substring(temp_length);
            }
            //解密
            StringBuilder temp = new StringBuilder();
            ciphertext.Add(str);
            int[] position = new int[temp_length];
            hash(ref position , temp_length , key);
            foreach (string t in ciphertext)
            {
                temp.Append(decryption_string(t , temp_length , hstr , key , position));
            }
            return temp.ToString();
        }
        /// <summary>
        /// 确认密码
        /// </summary>
        /// <param name="keystr">密码加密</param>
        /// <param name="key">要确认密码</param>
        /// <returns></returns>
        public bool confirm(string keystr , string key)
        {
            return string.Equals(decryption(keystr , key) , n_md5(key));
        }

        private string encryption_string(string str , int temp_length , string hstr , string key , int[] position)
        {
            //缓冲区
            char[] temp = new char[temp_length];
            //添加后缀
            str = str + hstr;
            //哈希
            for (int i = 0; i < str.Length; i++)
            {
                temp[position[i]] = str[i];
            }
            //填补空白
            fill_gaps(ref temp , key);
            //加密字符
            for (int i = 0; i < temp_length; i++)
            {
                temp[i] = encryption_character(temp[i] , key , i);
            }
            return new string(temp);

            /*
            //缓冲区
            char[] temp = new char[temp_length];
            //添加后缀
            str += hstr;

            //哈希
            //hash(str , ref temp , key);
            //int[] position = new int[temp_length];
            //hash(ref position , temp_length , key);

            for (int i = 0; i < str.Length; i++)
            {
                temp[position[i]] = str[i];
            }
            //填补空白
            fill_gaps(ref temp , key);
            //加密字符
            //foreach (char t in temp)
            for (int i = 0; i < temp_length; i++)
            {
                temp[i] = encryption_character(temp[i] , key , i);
            }
            return new string(temp);
            */
        }
        private string decryption_string(string str , int temp_length , string hstr , string key , int[] position)
        {
            //申请缓冲区
            char[] tempclearstr = new char[temp_length];
            char[] tempciphertext = new char[temp_length];
            //解密字符
            for (int i = 0; i < str.Length; i++)
            {
                tempciphertext[i] = decryption_character(str[i] , key , i);
            }
            //哈希函数
            for (int i = 0; i < temp_length; i++)
            {
                tempclearstr[i] = tempciphertext[position[i]];
            }
            //去后缀
            string
            temp = new string(tempclearstr);

            int count;
            count = temp.IndexOf(hstr);
            if (count > 0)
            {
                temp = temp.Substring(0 , count);
            }
            else
            {
                return temp;
            }
            return temp;

            /*
            //申请缓冲区
            char[] temp = new char[temp_length];
            char[] clearstr = new char[temp_length];
            //解密字符
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            for (int i = 0; i < temp_length; i++)
            {
                temp[i] = decryption_character(str[i] , key , i);
            }
            //哈希函数
            //int[] position = new int[temp_length];
            //hash(ref position , temp_length , key);
            for (int i = 0; i < temp_length; i++)
            {
                // temp[i] = str[position[i]];2015年10月30日15:10:11
                clearstr[i] = temp[position[i]];
            }
            //去后缀

            string t = new string(temp);
            int point = t.LastIndexOf(hstr);
            if (point > 0)
            {
                return t.Substring(0 , point);
            }
            else
            {
                return t;
            }*/
        }
        private void hash(ref int[] position , int temp_length , string key)
        {
            bool[] exist = new bool[temp_length];
            int i;
            int temp;
            int count;//冲突数
            int sum;//总冲突数
            for (i = 0; i < temp_length; i++)
            {
                exist[i] = false;
            }

            if (position.Length != temp_length)
            {
                position = new int[temp_length];
            }
            sum = 0;
            //一元n次函数
            for (i = 0; i < temp_length; i++)
            {
                temp = (int)( key[i % key.Length] ) % temp_length;
                count = i / key.Length;
                while (exist[temp])
                {
                    temp = conflict(temp , i , count , key , temp_length);
                    count++;
                    sum++;
                    if (count > 2 * temp_length)
                    {
                        Debug.Write("冲突数过多" + i.ToString() + "\r\n");
                        return;
                    }
                }
                position[i] = temp;
                exist[temp] = true;
            }
            Debug.Write("冲突数" + sum.ToString());

        }
        private int conflict(int position , int i , int count , string key , int temp_length)
        {
            int countconflict;
            int sum;
            i = i % key.Length;
            sum = 0;
            for (countconflict = 0; countconflict <= count; countconflict++)
            {
                sum += key[i];
            }
            sum += key[i];
            return sum % temp_length;
        }
        private char encryption_character(char str , string key , int i)
        {
            //return str;
            i = i % key.Length;
            return (char)( str + key[i] % 10 );
        }
        private char decryption_character(char str , string key , int i)
        {
            //return str;
            i = i % key.Length;
            return (char)( str - key[i] % 10 );
        }
        private void fill_gaps(ref char[] temp , string key)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == 0)
                {
                    temp[i] = (char)ran_char;
                }
            }
        }


        public string encryption_string(string str)
        {
            char[] temp_str = new char[_temp_string_lenth];
            int i, has, key_place;//has字符位置，key_place密码位置
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
                temp_str[has] = (char)( ( str[i] ) + key[key_place] % 1024 );
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
                    temp_str[i] = Convert.ToChar(ran_char); //% 1000+1);
                }
            }
            return new string(temp_str);
            //string s = new string(temp_str);
            //return s;
            //return null;
        }
        public string decryption_string(string str)
        {
            StringBuilder temp = new StringBuilder();
            char[] jie = str.ToCharArray();
            int has, key_place;//has字符位置，key_place密码位置
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
                temp.Append((char)( ( jie[has] ) - key[key_place] % 1024 ));
                jie[has] = Convert.ToChar(0);//把原来位置0
                key_place++;
                if (key_place == key.Length)
                {
                    key_place = 0;
                }
                if (temp.Length == _temp_string_lenth)
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
            if (temp_l == 0)
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
        private static string_encryption _string_decryption = new string_encryption();
        private Random _random = new Random();
        //加密文件的路径
        //private string _file_address;
        private string _key;
        private string _后缀;

        private int ran_char
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
            Windows.Security.Cryptography.Core.HashAlgorithmProvider objAlgProv = Windows.Security.Cryptography.Core.HashAlgorithmProvider.OpenAlgorithm(Windows.Security.Cryptography.Core.HashAlgorithmNames.Md5);
            Windows.Security.Cryptography.Core.CryptographicHash md5 = objAlgProv.CreateHash();
            Windows.Storage.Streams.IBuffer buffMsg1 = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(str , Windows.Security.Cryptography.BinaryStringEncoding.Utf16BE);
            md5.Append(buffMsg1);
            Windows.Storage.Streams.IBuffer buffHash1 = md5.GetValueAndReset();
            return Windows.Security.Cryptography.CryptographicBuffer.EncodeToBase64String(buffHash1);

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
        }
        private int range(int a , int b , int up , int down)
        {
            int r = up;
            int l = down;
            int mod = r - l + 1;
            if (a > 0 && b > 0)
            {
                return ( a % mod + b % mod + mod ) % mod + l;
            }
            else if (a > 0 && b < 0)
            {
                return ( a % mod - b % mod + mod ) % mod + l;
            }
            else
            {
                return a + b;
            }
        }
    }

}
