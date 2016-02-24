// lindexi
// 9:24

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
        }

        public encryption_note(StorageFile file)
        {
            _file = file;
            if (file != null)
            {
                file_address = file.Name;
            }
        }

        public bool read_encryption { set; get; }

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
        private string_encryption encryption { set; get; } = new string_encryption();

        private string _encryption_key;

        private string _encryption_text;
        private StorageFile _file;
        private string _file_address;
        private bool _read_key;
        private IAsyncAction _storage;

        public bool confirm(string key)
        {
            
            return encryption.confirm(encryption_key,key)
        }


        /// <summary>
        /// 保存
        /// </summary>
        public void storage()
        {
            //motify = true;
            //if (!motify)
            //{
            //    return;
            //}

            //motify = false;

            _storage?.Cancel();

            string str = encryption_key + encryption_text;
            _storage = ThreadPool.RunAsync(async (workItemHandler) =>
            {
                await file_null();

                using (StorageStreamTransaction transaction = await _file.OpenTransactedWriteAsync())
                {
                    using (DataWriter data_writer = new DataWriter(transaction.Stream))
                    {
                        data_writer.WriteString(str);
                        transaction.Stream.Size = await data_writer.StoreAsync();
                        await transaction.CommitAsync();
                    }
                }
            });
        }

        private async Task file_null()
        {
            if (_file == null)
            {
                _file =
                    await
                        ApplicationData.Current.
                            LocalFolder.CreateFileAsync(file_address,
                                CreationCollisionOption.OpenIfExists);
            }
        }

        private async void read()
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
                        encryption_text = data_reader.ReadString(num_bytes_loaded);
                    }
                }
            }
        }
    }
}