using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace encryption_code_book.Model
{
    public class EncryCodeSecretScribe
    {
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

        private new void Storage()
        {
            EncryCodeSecretScribe encryCodeSecretScribe = new EncryCodeSecretScribe()
            {
                Name = "ÀΩ√‹√‹¬Î±æ 2",
                ComfirmKey = ComfirmKey
            };

        }

        public ObservableCollection<SecretScribeCode> SecretCode
        {
            set;
            get;
        }=new ObservableCollection<SecretScribeCode>();

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

        private bool _check;
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

        public async Task Read()
        {
            string str = "data.encry";
            var file = await EncryCodeFolder.GetFileAsync(str);
            str = await FileIO.ReadTextAsync(file);
            var json = JsonSerializer.Create();
            EncryCodeSecretScribe encryCodeSecretScribe =
                json.Deserialize<EncryCodeSecretScribe>(new JsonTextReader(new StringReader(str)));
            EncryCodeSecretScribe = encryCodeSecretScribe;
            OnRead?.Invoke(this,encryCodeSecretScribe);
        }

        [JsonIgnore]
        public EventHandler<EncryCodeSecretScribe> OnRead;
        [JsonIgnore]
        public EncryCodeSecretScribe EncryCodeSecretScribe
        {
            set;
            get;
        }

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
    }
}