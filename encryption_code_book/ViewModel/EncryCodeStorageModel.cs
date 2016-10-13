using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using encryption_code_book.Model;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace encryption_code_book.ViewModel
{
    public class EncryCodeStorageModel : NotifyProperty
    {
        public EncryCodeStorageModel()
        {
            ReadVisibility = Visibility.Visible;
            Read();
        }

        private Visibility _readVisibility;

        public Visibility ReadVisibility
        {
            set
            {
                _readVisibility = value;
                OnPropertyChanged();
            }
            get
            {
                return _readVisibility;
            }

        }

        public ObservableCollection<SecretScribe> EncryCodeStorage
        {
            set
            {
                _encryCodeStorage = value;
                OnPropertyChanged();
            }
            get
            {
                return _encryCodeStorage;
            }

        }

        private ObservableCollection<SecretScribe> _encryCodeStorage;

        private async void Read()
        {
            var account = AccountGoverment.View;

            if (account.EncryCodeStorage == null)
            {
                account.EncryCodeStorage = new List<SecretScribe>();

                foreach (var temp in await ReadFolder())
                {
                    try
                    {
                        SecretScribe secret = new SecretScribe
                        {
                            Token = temp.Token,
                            EncryCodeFolder = await
                                StorageApplicationPermissions.
                                    FutureAccessList.GetFolderAsync(temp.Token),
                            Name = temp.Name
                        };
                        
                        account.EncryCodeStorage.Add(secret);
                    }
                    catch
                    {

                    }
                }



            }

            foreach (var temp in account.EncryCodeStorage)
            {
                EncryCodeStorage.Add(temp);
            }

            ReadVisibility = Visibility.Collapsed;
        }

        private static async Task<List<SecretScribe>> ReadFolder()
        {
            try
            {
                string str = "encryCode.encry";
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(
                    str);
                str = await FileIO.ReadTextAsync(file);
                var json = JsonSerializer.Create();
                var temp = json.Deserialize<List<SecretScribe>>(new JsonTextReader(
                    new StringReader(str)));
                return temp;
            }
            catch (Exception)
            {
                
            }
            return new List<SecretScribe>();
        }
    }
}
