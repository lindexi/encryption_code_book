using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using encryption_code_book.ViewModel;
using Newtonsoft.Json;

namespace encryption_code_book.Model
{
    public class KeySecret : NotifyProperty
    {
        public KeySecret()
        {
            AreNewEncrypt = true;
        }

        [JsonIgnore]
        public Account Account { set; get; }

        public KeySecret(Account account)
        {
            AreNewEncrypt = true;
            Account = account;
        }

        [JsonIgnore]
        public string Key
        {
            set
            {
                _key = value;
                OnPropertyChanged();
            }
            get
            {
                return _key;
            }
        }

        private string _key;
        [JsonIgnore]
        public bool AreNewEncrypt
        {
            set;
            get;
        }

        public string Name
        {
            set;
            get;
        }

        [JsonIgnore]
        public byte[] ConfirmKey
        {
            set;
            get;
        }

        [JsonIgnore]
        public StorageFile File
        {
            set;
            get;
        }

        [JsonIgnore]
        public StorageFolder Folder { set; get; }

        public string Token
        {
            set;
            get;
        }

        public async Task Read()
        {
            if (Folder == null)
            {
                throw new FileNotFoundException();
            }
            var file = await Folder.GetFileAsync(Account.FacitFile);
            await Read(file);
        }

        public async Task Read(StorageFile file)
        {
            //读了确认
            try
            {
                CachedFileManager.DeferUpdates(file);
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    // 用来判断密码是否正确的 Key 长度
                    const int applicationHeaderLength = Account.ApplicationHeaderLength;
                    byte[] buffer = new byte[applicationHeaderLength];
                    stream.Read(buffer, 0, applicationHeaderLength);

                    string headerText = Account.Encod.GetString(buffer).Trim();
                    if (!headerText.Equals(Account.Serializer()))
                    {
                        //以前版本
                        await (new MessageDialog("发现以前版本，请使用以前版本软件打开")).ShowAsync();
                    }

                    // 看起来 Buffer 内容可以复用
                    const int confirmKeyLength = Account.ConfirmKeyLength;
#pragma warning disable 162
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (confirmKeyLength != applicationHeaderLength)
                    {
                        buffer = new byte[confirmKeyLength];
                    }
#pragma warning restore 162

                    stream.Read(buffer, 0, confirmKeyLength);
                    ConfirmKey = buffer;
                }
                FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
                if (updateStatus == FileUpdateStatus.Complete)
                {
                    File = file;
                    AreNewEncrypt = false;
                }
            }
            catch
            {

            }
        }

        public virtual async Task Storage()
        {
            string str = Account.Serializer().Trim();
            var n = Account.ConfirmKeyLength;
            var file = await Folder.CreateFileAsync(Account.FacitFile);
            byte[] buffer = new byte[n];

            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                Account.Encod.GetBytes(str).CopyTo(buffer, 0);
                stream.Write(buffer, 0, n);
                var key = new StringEncryption();
                ConfirmKey = key.Encryption(Key, Key);
                buffer = Account.Encod.GetBytes(str);
                stream.Write(buffer, 0, buffer.Length);

            }

        }
    }
}