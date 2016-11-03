// lindexi
// 19:13

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Core;
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

        public bool EncryCodeStorageEnable
        {
            set
            {
                if (value == _encryCodeStorageEnable)
                {
                    return;
                }
                _encryCodeStorageEnable = value;
                OnPropertyChanged();
            }
            get
            {
                return _encryCodeStorageEnable;
            }
        }

        public Visibility ReadVisibility
        {
            set
            {
                _readVisibility = value;
                EncryCodeStorageEnable = value == Visibility.Collapsed;
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

        public void NewEncryCodeStorage()
        {
            //AccountGoverment.View.Frame
            //添加后退
        }

        private ObservableCollection<SecretScribe> _encryCodeStorage;

        private bool _encryCodeStorageEnable;

        private Visibility _readVisibility;

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
                if(string.IsNullOrEmpty(temp.ComfirmKey))
                {
                    await temp.Read();
                }
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        EncryCodeStorage.Add(temp);
                    });
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