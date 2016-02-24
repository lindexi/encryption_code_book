// lindexi
// 19:45

#region

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;

#endregion

namespace encryption_code_book.Model
{
    public class encryption_note
    {
        public encryption_note()
        {
            //motify = false;
            file_address = "data.encryption";
            file_null().Wait();
            _read = read();
        }

        public encryption_note(StorageFile file)
        {
            _file = file;
            if (file != null)
            {
                file_address = file.Name;
            }
        }

        public bool read_encryption
        {
            set;
            get;
        }

        public string encryption_key
        {
            set
            {
                _encryption_key = value;
            }
            get
            {
                return _encryption_key;
            }
        }

        public string encryption_text
        {
            set
            {
                _encryption_text = value;
            }
            get
            {
                return _encryption_text;
            }
        }

        /// <summary>
        /// 密码保存的文件读取
        /// </summary>
        public bool read_key
        {
            get
            {
                return _read_key;
            }
            set
            {
                _read_key = value;
            }
        }

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

        //private bool motify;
        private string_encryption encryption
        {
            set;
            get;
        } = new string_encryption();

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

        public string key
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

        private string _encryption_key;

        private string _encryption_text;
        private StorageFile _file;
        private string _file_address;
        private bool _first;
        private string _key;
        private Task _read;
        private bool _read_key;
        private IAsyncAction _storage;

        public void new_use(string keystr)
        {
            storage(keystr);
        }

        public bool confirm(string keystr)
        {
            if (string.IsNullOrEmpty(encryption_key))
            {
                if (_read == null)
                {
                    _read = read();
                }
                _read.Wait();
            }
            key = keystr;
            return encryption.confirm(encryption_key, keystr);
        }


        /// <summary>
        /// 保存
        /// </summary>
        public void storage(string str)
        {
            //motify = true;
            //if (!motify)
            //{
            //    return;
            //}

            //motify = false;

            _storage?.Cancel();

            _storage = ThreadPool.RunAsync(async (workItemHandler) =>
            {
                await file_null();

                using (StorageStreamTransaction transaction = await _file.OpenTransactedWriteAsync())
                {
                    using (DataWriter data_writer = new DataWriter(transaction.Stream))
                    {
                        data_writer.WriteString(serialization(str));
                        transaction.Stream.Size = await data_writer.StoreAsync();
                        await transaction.CommitAsync();
                    }
                }
            });
        }

        private string serialization(string str)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = "qq123456";
            }

            encryption_key = encryption.encryption(encryption.n_md5(key), key);

            encryption_text = encryption.encryption(str, key);
            str = @"私密密码本
作者:lindexi_gd
邮箱:lindexi_gd@163.com
哈希路径:一元n次函数
步长:加密钥unicode，冲突过多使用步长1
字符加密:密钥模10
缓冲区长度:1024".PadRight(1024);
            return str + encryption_key + encryption_text;
        }

        private string deserilization(string str)
        {
            int temp_length = 1024;
            if (str.Length < temp_length)
            {
                //throw new Exception();
                first = true;
                return string.Empty;
            }
            string temp = str.Substring(0, temp_length);

            encryption_key = str.Substring(temp_length, temp_length*2);
            encryption_text = str.Substring(temp_length*2);
            return "";
        }

        private async Task file_null()
        {
            if (_file == null)
            {
                try
                {
                    _file = await ApplicationData.Current.
                        LocalFolder.GetFileAsync(file_address);
                }
                catch
                {
                    _file =
                        await
                            ApplicationData.Current.
                                LocalFolder.CreateFileAsync(file_address);
                    first = true;
                }
            }
        }

        private async Task read()
        {
            await file_null();

            using (IRandomAccessStream read_stream = await _file.OpenAsync(FileAccessMode.Read))
            {
                using (DataReader data_reader = new DataReader(read_stream))
                {
                    ulong size = read_stream.Size;
                    if (size <= uint.MaxValue)
                    {
                        uint num_bytes_loaded = await data_reader.LoadAsync((uint) size);
                        deserilization(data_reader.ReadString(num_bytes_loaded));
                    }
                }
            }
        }
    }
}