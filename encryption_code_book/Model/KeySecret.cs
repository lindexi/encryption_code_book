using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
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
        public string Key
        {
            set;
            get;
        }
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
        
        public string Token
        {
            set;
            get;
        }

        public async Task Read(StorageFile file)
        {
            //∂¡¡À»∑»œ
            try
            {
                byte[] buffer = new byte[AccountGoverment.View.Account.ComfirmkeyLength];

                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    stream.Read(buffer, 0, AccountGoverment.View.Account.ComfirmkeyLength);
                }

                ComfirmKey = AccountGoverment.View.Account.Encod.GetString(buffer);
                File = file;
                AreNewEncrypt = false;
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