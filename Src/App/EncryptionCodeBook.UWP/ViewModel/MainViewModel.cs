using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using encryption_code_book.Model;
using encryption_code_book.View;
using Framework.ViewModel;
using Newtonsoft.Json;
using FileIO = encryption_code_book.Model.FileIO;

namespace encryption_code_book.ViewModel
{
    public class MainViewModel : NavigateViewModel
    {
        public MainViewModel()
        {
            Account = new Account();
        }

        //public CodeModel CodeModel { get; set; }

        //public AModel AModel
        //{
        //    set;
        //    get;
        //}

        //public LinModel LinModel
        //{
        //    set;
        //    get;
        //}

        //public CodeStorageModel CodeStorageModel
        //{
        //    set;
        //    get;
        //}

        public Visibility FrameVisibility
        {
            set
            {
                _frameVisibility = value;
                OnPropertyChanged();
            }
            get
            {
                return _frameVisibility;
            }
        }

        public MainViewModel MainView
        {
            set;
            get;
        }



        public void NavigateToInfo()
        {
        }

        public void NavigateToAccount()
        {
        }

        public override void OnNavigatedFrom(object obj)
        {

        }

        public override void OnNavigatedTo(object obj)
        {
            FrameVisibility = Visibility.Collapsed;
            Content = (Frame)obj;
#if NOGUI
#else
            Content.Navigate(typeof(SplashPage));
#endif
            if (ViewModel == null)
            {
                ViewModel = new List<ViewModelPage>();
                //加载所有ViewModel
                var applacationAssembly = Application.Current.GetType().GetTypeInfo().Assembly;

                foreach (var temp in applacationAssembly.DefinedTypes.Where(temp => temp.IsSubclassOf(typeof(ViewModelBase))))
                {
                    ViewModel.Add(new ViewModelPage(temp.AsType()));
                }

                foreach (var temp in applacationAssembly.DefinedTypes.Where(temp => temp.IsSubclassOf(typeof(Page))))
                {
                    //获取特性，特性有包含ViewModel
                    var p = temp.GetCustomAttribute<ViewModelAttribute>();

                    var viewmodel = ViewModel.FirstOrDefault(t => t.Equals(p?.ViewModel));
                    if (viewmodel != null)
                    {
                        viewmodel.Page = temp.AsType();
                    }
                }

                Composite = new List<Composite>();
                foreach (var temp in applacationAssembly.DefinedTypes.Where(temp => temp.IsSubclassOf(typeof(Composite))))
                {
                    Composite.Add((Composite)temp.AsType().GetConstructor(Type.EmptyTypes)?.Invoke(null));
                }
            }

            Read();
        }
        public Account Account
        {
            set;
            get;
        }
        //public KeySecret Key
        //{
        //    set { Account.Key = value; }
        //    get { return Account.Key; }
        //}



        private async void Read()
        {
            if (Account.Key == null)
            {
                Account.Key = new KeySecret(Account);


                try
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    try
                    {
                        var file = await folder.GetFileAsync(
                            Account.FacitFile);
                        Account = await Account.Read(file);
                    }
                    catch (FileNotFoundException)
                    {
                        //file = await folder.CreateFileAsync(Account.FacitFile);
                        //string str = JsonConvert.SerializeObject(Account);
                        //await FileIO.WriteTextAsync(file, str);
                    }
                }
                catch
                {

                }
            }

            //文件夹


            FrameVisibility = Visibility.Visible;
            if (Account.Key.AreNewEncrypt)
            {
                Navigate(typeof(NewCodeStorageModel), null);
            }
            else
            {
                Navigate(typeof(KeyModel), null);
            }
        }

        //private async Task SerializerKeySecret(StorageFile file)
        //{
        //    CachedFileManager.DeferUpdates(file);

        //    string str = await FileIO.ReadTextAsync(file);
        //    var account = JsonConvert.DeserializeObject<Account>(str);
        //    if (account == null)
        //    {
        //        throw new Exception();
        //    }
        //    var key = account.Key;
        //    key.Folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(key.Token);
        //    Account = account;

        //    FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
        //}


        private Visibility _frameVisibility;

    }
}