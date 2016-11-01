// lindexi
// 18:49

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using encryption_code_book.ViewModel;
using Newtonsoft.Json;

namespace encryption_code_book.Model
{
    public class EncryCodeSecretScribe
    {
        public EncryCodeSecretScribe()
        {
            Encry = AccountGoverment.View.Account.Encry;
        }

        public EncryCodeSecretScribe(string name, string comfirmKey)
        {
            Name = name;
            ComfirmKey = comfirmKey;
        }

        public string Name
        {
            set;
            get;
        }

        public string ComfirmKey
        {
            set;
            get;
        }

        public string Encry
        {
            set;
            get;
        }
    }

    public class SecretScribeCode
    {
        public string Name
        {
            set;
            get;
        }

        public string Str
        {
            set;
            get;
        }
    }

    public class SecretScribe : KeySecret
    {
        public SecretScribe()
        {
        }

        public ObservableCollection<SecretScribeCode> SecretCode
        {
            set;
            get;
        } = new ObservableCollection<SecretScribeCode>();

        //public SecretScribe(StorageFolder folder)
        //{
        //    EncryCodeFolder = folder;
        //    Name = folder.Name;

        //    //Read();
        //}

        [JsonIgnore]
        public StorageFolder EncryCodeFolder
        {
            set;
            get;
        }

        [JsonIgnore]
        public bool Check
        {
            set
            {
                _check = value;
                OnPropertyChanged();
            }
            get
            {
                return _check;
            }
        }

        [JsonIgnore]
        public EncryCodeSecretScribe EncryCodeSecretScribe
        {
            set;
            get;
        }

        [JsonIgnore]
        public string Str
        {
            set
            {
                _str = value;
                OnPropertyChanged();
            }
            get
            {
                return _str;
            }
        }

        public async Task Read()
        {
            try
            {
                string str = "data.encry";
                var file = await EncryCodeFolder.GetFileAsync(str);
                str = await FileIO.ReadTextAsync(file);
                var json = JsonSerializer.Create();
                EncryCodeSecretScribe encryCodeSecretScribe =
                    json.Deserialize<EncryCodeSecretScribe>(new JsonTextReader(new StringReader(str)));
                //EncryCodeSecretScribe = encryCodeSecretScribe;
                Name = encryCodeSecretScribe.Name;
                ComfirmKey = encryCodeSecretScribe.ComfirmKey;

                OnRead?.Invoke(this, true);
            }
            catch (Exception)
            {
                OnRead?.Invoke(this, false);
            }
        }

        private bool _check;

        //private string _name;

        //public string Name
        //{
        //    set
        //    {
        //        _name = value;
        //        OnPropertyChanged();
        //    }
        //    get
        //    {
        //        return _name;
        //    }
        //}

        private string _str;

        [JsonIgnore]
        public EventHandler<bool> OnRead;

        [JsonIgnore]
        public EventHandler<bool> OnStorage;

        public new async Task Storage()
        {
            try
            {
                EncryCodeSecretScribe encryCodeSecretScribe = new EncryCodeSecretScribe()
                {
                    Name = Name,
                    ComfirmKey = ComfirmKey
                };

                string str = "data.encry";
                StorageFile file;
                try
                {
                    file = await EncryCodeFolder.GetFileAsync(str);
                }
                catch (FileNotFoundException)
                {
                    file = await EncryCodeFolder.CreateFileAsync(str);
                }

                str = JsonConvert.SerializeObject(encryCodeSecretScribe);
                await FileIO.WriteTextAsync(file, str);
                OnStorage?.Invoke(this, true);
            }
            catch (Exception)
            {
                OnStorage?.Invoke(this, false);
            }
        }
    }
}