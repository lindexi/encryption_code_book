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
        private Account Account { get; }

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
        public string ComfirmKey
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

        public async Task Read(StorageFile file)
        {
            //读了确认
            try
            {
                CachedFileManager.DeferUpdates(file);
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, 1024);
                    string comfirm = Account.Encod.GetString(buffer).Trim();
                    if (!comfirm.Equals(Account.Serializer()))
                    {
                        //以前版本
                        await (new MessageDialog("发现以前版本，请使用以前版本软件打开")).ShowAsync();
                    }
                    ComfirmKey = Account.Encod.GetString(buffer);
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

        public void Storage()
        {

        }
    }
}