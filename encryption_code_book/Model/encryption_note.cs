// lindexi
// 15:19

#region

using System;
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
            motify = false;
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

        private string _encryption_key;

        private string _encryption_text;
        private StorageFile _file;
        private bool _read_key;
        private IAsyncAction _storage;
        private bool motify;

        /// <summary>
        /// 保存
        /// </summary>
        public void storage()
        {
            motify = true;
            if (!motify)
            {
                return;
            }

            motify = false;

            _storage?.Cancel();

            string str = encryption_key + encryption_text;
            _storage = ThreadPool.RunAsync(async (workItemHandler) =>
            {
                if (_file == null)
                {
                    string file_address = "data.encryption";
                    _file =
                        await
                            ApplicationData.Current.LocalFolder.CreateFileAsync(file_address,
                                CreationCollisionOption.OpenIfExists);
                }

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

        private async void read()
        {
            if (_file == null)
            {
                string file_address = "data.encryption";
                _file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync(file_address,
                            CreationCollisionOption.OpenIfExists);
            }

            using (IRandomAccessStream read_stream = await _file.OpenAsync(FileAccessMode.Read))
            {
                using (DataReader data_reader = new DataReader(read_stream))
                {
                    ulong size = read_stream.Size;
                    if (size <= uint.MaxValue)
                    {
                        uint numBytesLoaded = await data_reader.LoadAsync((uint) size);
                        encryption_text = data_reader.ReadString(numBytesLoaded);
                    }
                }
            }
        }
    }
}