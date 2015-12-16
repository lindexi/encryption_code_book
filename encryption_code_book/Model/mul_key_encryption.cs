using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace encryption_code_book
{
    public class mul_key_encryption
    {
        public mul_key_encryption()
        {
            Task d;
            d = new_class();
        }
        public mul_key_encryption(string key)
        {
            Task b = new_class(key);
        }

        ~mul_key_encryption()
        {
            file_key.Clear();
            decryption.Clear();


        }

        /// <summary>
        /// 默认没有jia.key 密码
        /// </summary>
        public List<string> file_key
        {
            set
            {
                _key = value;
            }
            get
            {
                return _key;
            }
        }
        public List<string> decryption
        {
            set
            {
                _decryption = value;
            }
            get
            {
                return _decryption;
            }
        }
        public string file_address
        {
            set
            {
                jia.file_address = value;
            }
            get
            {
                return jia.file_address;
            }
        }
        /// <summary>
        /// 第一次运行
        /// </summary>
        public bool first
        {
            set
            {
                _first = value;
            }
            get
            {
                return _first;
            }
        }
        public StorageFile file
        {
            set
            {
                _file = value;
            }
            get
            {
                return _file;
            }
        }

        /// <summary>
        /// 读写
        /// </summary>
        public async Task<string> ce()
        {
            string str = "asdsfesrsdfsfse";
            file_address = "12data.encryption";
            decryption.Add(str);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                file = await folder.GetFileAsync(file_address);
            }
            catch (FileNotFoundException e)
            {
                str = e.ToString();
            }
            //file = await sf.OpenStreamForWriteAsync();
            await x_write();
            using (Stream filestream = await file.OpenStreamForReadAsync())
            {
                byte[] buff = new byte[1000];
                if (filestream.Read(buff , 0 , 1000) > 0)
                {
                    return Encoding.Unicode.GetString(buff);
                }
            }

            using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
            {
                using (DataWriter dataWriter = new DataWriter(transaction.Stream))
                {
                    dataWriter.WriteString(str);
                    transaction.Stream.Size = await dataWriter.StoreAsync();
                    await transaction.CommitAsync();
                }
            }

            using (IRandomAccessStream readstream = await file.OpenAsync(FileAccessMode.Read))
            {
                using (DataReader dataRead = new DataReader(readstream))
                {
                    UInt64 size = readstream.Size;
                    UInt32 numBytesLoaded = await dataRead.LoadAsync((UInt32)size);
                    str = dataRead.ReadString(numBytesLoaded);
                }
            }
            if (!string.IsNullOrEmpty(str))
            {
                return str;
            }

            //file = await folder.OpenStreamForWriteAsync(file_address , CreationCollisionOption.OpenIfExists);
            //await x_write();

            return str;
        }
        public async Task<bool> confirm_password(string key)
        {
            if (string.IsNullOrEmpty(key_encry))
            {
                await d;
            }
            jia.key = key;
            //string l1, l2;
            //l1 = jia.n_md5(key);
            //l2 = jia.decryption(key_encry);
            //return string.Equals(l1 , l2);

            //int len;
            //string temp;
            //len = 2048;
            return string.Equals(jia.n_md5(key) , jia.decryption(key_encry));
            //FileStream fs = new FileStream(file_address , FileMode.Open);
            //using (Stream fs = await file.OpenStreamForReadAsync())
            //{
            //    byte[] buffer = new byte[len];
            //    fs.Read(buffer , 0 , len);//不密码
            //                              //fs.Close();
            //    temp = Encoding.Unicode.GetString(buffer);
            //    jia.key = key;
            //    //string l1,l2;
            //    //l1 = jia.n_md5(key);
            //    //l2 = jia.decryption(temp);
            //    //return string.Equals(l1 , l2);
            //    return string.Equals(jia.n_md5(key) , jia.decryption(temp));
            //}
        }
        /// <summary>
        /// 获得密码
        /// </summary>
        /// <returns>保存下密码</returns>
        public async Task<string> jmz_decryption()
        {
            StringBuilder str = new StringBuilder();
            //string str;
            //str = "";
            file_key.Clear();
            await d;
            //key_encry = string.Empty;            

            foreach (string temp in decryption)
            {
                file_key.Add(jia.decryption(temp));
            }
            foreach (string temp in file_key)
            {
                str.Append(temp);//之前有修一个temp最大为100 + "\r\n\r\n"
            }
            return str.ToString();
        }
        /// <summary>
        /// 要新增密码
        /// </summary>
        /// <param name="key">密码</param>
        public async void new_entryption(string key)
        {
            int sub = key.IndexOf("\r\n\r\n");
            int max_count;//一个密码长度
            string temp;
            max_count = 100;
            while (sub != -1)
            {
                //如果开头\r\n\r\n那么直接去空
                temp = key.Substring(0 , sub);
                if (temp.Length > 0)
                {
                    while (temp.Length > max_count)
                    {
                        file_key.Add(temp.Substring(0 , max_count));
                        temp = temp.Substring(max_count);
                    }
                    file_key.Add(temp);
                }
                key = key.Substring(sub + 4);
                sub = key.IndexOf("\r\n\r\n");
            }
            file_key.Add(key);
            //加密();
            await hold_decryption_key();
        }
        /// <summary>
        /// 覆盖密码，把原有密码删掉
        /// </summary>
        /// <param name="key">密码</param>
        public async Task override_entryption(string key)
        {
            file_key.Clear();//污染
            while (key.Length > 100)
            {
                file_key.Add(key.Substring(0 , 100));
                key = key.Substring(100);
            }
            file_key.Add(key);
            await hold_decryption_key();
            //if(file_key.Count<=0)
            //{
            //    encryption.main(file_key.Count);
            //}
        }

        public async void new_use(string key,AsyncCallback call_back)
        {
            jia.key = key;
            file_key.Clear();
            decryption.Clear();
            first = false;

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            file = await folder.CreateFileAsync(file_address , CreationCollisionOption.OpenIfExists);

            await hold_decryption_key();
            //x_write();2015年5月12日19:55:57
            call_back(new async_result(true));
        }

        /// <summary>
        /// 删除密码
        /// </summary>
        /// <param name="key">要删除密码</param>
        /// <returns>如果删除return true;如果没有return false</returns>
        public bool delete_key(string key)
        {
            return file_key.Remove(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">密码号</param>
        /// <returns></returns>
        public bool delete_key(int key)
        {
            if (key < file_key.Count)
            {
                file_key.RemoveAt(key);
                return true;
            }
            return false;
        }

        private string_decryption jia
        {
            set
            {
                value = null;
            }
            get
            {
                return string_decryption.g_获得类();
            }
        }

        private StorageFile _file;
        private string key_encry;
        private Task d;

        private List<string> _key = new List<string>();

        private List<string> _decryption = new List<string>();
        private bool _first;
        //private enedrvvmpyyxygdnxgmhjzxbmt encryption=new enedrvvmpyyxygdnxgmhjzxbmt();

        private async Task<bool> new_class()
        {
            await new_class("qq123456");
            return true;
        }
        private async Task<bool> new_class(string key)
        {
            file_address = @"data.encryption";
            jia.file_address = file_address;
            jia.key = key;
            first = false;
            //本地没有出现 file address
            bool localFolder_find_file_address;
            //漫游没有出现 file address
            bool roamingFolder_find_file_address;
            localFolder_find_file_address = false;
            roamingFolder_find_file_address = false;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(file_address);
            }
            catch //(FileNotFoundException e)
            {
                localFolder_find_file_address = true;
               
            }
            if (localFolder_find_file_address)
            {
                try
                {
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(file_address);
                }
                catch //(FileNotFoundException e)
                {
                    roamingFolder_find_file_address = true;
                }
            }
            first = localFolder_find_file_address && roamingFolder_find_file_address;
            if (!first)
            {
                d = d_scan();
            }
            return !first;
        }

        /// <summary>
        /// 写加密文件 storage 2015年10月5日15:06:42
        /// </summary>
        ///<param name="str">函数要写入文件内容</param>
        /// <returns>如果写成功返回true</returns>
        private async Task<bool> x_write()
        {
            using (Stream xiaFile = await file.OpenStreamForWriteAsync())
            {
                byte[] buf;
                foreach (string temp in decryption)
                {
                    buf = Encoding.Unicode.GetBytes(temp);
                    xiaFile.Write(buf , 0 , buf.Length);
                }
                //decryption.Clear();  
                decryption.RemoveAt(0);
                xiaFile.Flush();
                //xiaFile.Close();
                return true;
            }
        }
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="str">读文件内容保存到str</param>
        /// <returns>读文件成功返回true</returns>
        private async Task<bool> d_scan()
        {
            string temp;
            int len;//文件长
            len = 2048;
            //文件流
            //try
            {
                using (Stream fs = await file.OpenStreamForReadAsync())
                {
                    decryption.Clear();//不污染
                    //FileStream fs = new FileStream(file_address , FileMode.Open);
                    //缓冲
                    byte[] buffer = new byte[len];
                    fs.Read(buffer , 0 , len);//不密码
                    //lock (key_encry)
                    {
                        key_encry = Encoding.Unicode.GetString(buffer);
                    }
                    while (fs.Read(buffer , 0 , len) != 0)
                    {
                        temp = Encoding.Unicode.GetString(buffer);
                        //没有解密
                        decryption.Add(temp);
                    }
                    //让文件可以被其他打开
                    //fs.Close();
                    return true;
                }
            }
            //catch
            //{
            //    //如果文件被占用，没有找到文件
            //    return false;
            //}
        }

        /// <summary>
        /// 添加、删除，可以保存密码
        /// </summary>
        private async Task hold_decryption_key()
        {
            //会污染
            decryption.Clear();
            //启动密码
            decryption.Add(jia.encryption(jia.n_md5(jia.key)));
            foreach (string temp in file_key)
            {
                if (temp != null && temp.Length > 0)
                {
                    decryption.Add(jia.encryption(temp));
                }
                //如果密码空，不保存
            }
            await x_write();
        }
    }
    

}
