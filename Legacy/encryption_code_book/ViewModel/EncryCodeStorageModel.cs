// lindexi
// 19:19

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
using Windows.UI.Xaml.Controls;
using encryption_code_book.Model;
using encryption_code_book.View;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using FileIO = encryption_code_book.Model.FileIO;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace encryption_code_book.ViewModel
{
    public class EncryCodeStorageModel : ViewModelBase
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
            Frame frame = AccountGoverment.View.Frame;
            frame.Navigate(typeof(NewCodeStoragePage));
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
                if (string.IsNullOrEmpty(temp.ComfirmKey))
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

        public override void OnNavigatedFrom(object obj)
        {
        }

        public override void OnNavigatedTo(object obj)
        {
        }

        public override void Receive(object source, Message message)
        {
        }
    }
}