using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Provider;
using Newtonsoft.Json;

namespace encryption_code_book.Model
{
    public class Account
    {
        public Account()
        {

        }


        public KeySecret Key
        {
            set;
            get;
        }

        public string Folder
        {
            get;
        } = "encryption";

        public string Patched
        {
            get;
        } = ".encry";

        public string FacitFile
        {
            get;
        } = "encryption.encry";

        public string EncryptionCodeNoteFolder
        {
            get;
        } = "EncryptionCode";

        public const string Encry
         = "私密密码本 2";

        public static string Serializer()
        {
            string str = Encry + "\r\n" +
                         "林德熙" + "\r\n" +
                         ComfirmkeyLength + "\r\n";
            return str.PadRight(1024);
        }

        public const int ComfirmkeyLength
        = 1024;

        public Encoding Encod
        {
            set;
            get;
        } = Encoding.Unicode;

        public static async Task<Account> Read(StorageFile file)
        {
            CachedFileManager.DeferUpdates(file);

            string str = await FileIO.ReadTextAsync(file);
            var comfirm = str.Substring(0, 1024);
            if (comfirm == Serializer())
            {
                
            }
            str = str.Substring(1024);

            FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);

            var account = JsonConvert.DeserializeObject<Account>(str);
            if (account == null)
            {
                throw new Exception();
            }
            var key = account.Key;
            if (string.IsNullOrEmpty(key.Token))
            {
                key.AreNewEncrypt = true;
            }
            else
            {
                key.Folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(key.Token);
            }

            return account;
        }

        public async Task Storage()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(FacitFile,CreationCollisionOption.ReplaceExisting);
            string str = JsonConvert.SerializeObject(this);
            str = Serializer() + str;
            CachedFileManager.DeferUpdates(file);
            await FileIO.WriteTextAsync(file, str);
            FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
        }
    }
}