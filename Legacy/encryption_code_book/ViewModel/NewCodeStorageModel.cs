using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using encryption_code_book.Model;

namespace encryption_code_book.ViewModel
{
    public class NewCodeStorageModel : ViewModelBase
    {
        public NewCodeStorageModel()
        {

        }

        private string _name;

        public string Name
        {
            set
            {
                _name = value;
                OnPropertyChanged();
            }
            get
            {
                return _name;
            }
        }

        private string _comfirmKey;


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

        public string ComfirmKey
        {
            set
            {
                _comfirmKey = value;
                OnPropertyChanged();
            }
            get
            {
                return _comfirmKey;
            }
        }

        public StorageFolder EncryCodeFolder
        {
            set
            {
                _encryCodeFolder = value;
                OnPropertyChanged();
            }
            get
            {
                return _encryCodeFolder;
            }
        }

        private StorageFolder _encryCodeFolder;


        private string _str;

        public void NewCodeStorage()
        {
            if (Comfirm())
            {
                //Send
                SecretCode key =new SecretCode();
                key.Str = Str;
                key.Key = ComfirmKey;
                key.Name = Name;
                key.Folder = EncryCodeFolder;
                Send?.Invoke(this,new NewCodeStorageMessage(this)
                {
                    Secret = key
                });
            }
        }

        private bool Comfirm()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }
            if (ComfirmKey.Length <= 3 || ComfirmKey.Length >= 30)
            {
                return false;
            }
            return true;
        }

        public void NewEncryCodeFolder()
        {

        }

        public void NavigateEncryCodeStoragePage()
        {

        }

        public override void OnNavigatedFrom(object obj)
        {
            Name = string.Empty;
            ComfirmKey = string.Empty;

        }

        public override void OnNavigatedTo(object obj)
        {
            //Key = (KeySecret)obj;
            Name = "默认密码本";
        }

        public override void Receive(object source, Message message)
        {
        }
    }

    internal class NewCodeStorageMessage:Message
    {
        public NewCodeStorageMessage(ViewModelBase source) : base(source)
        {

        }

        public SecretCode Secret { set; get; }
    }

    internal class CodeStorageComposite : Composite
    {
        public CodeStorageComposite()
        {
            Message = typeof(NewCodeStorageMessage);
        }

        public override async void Run(object sender, Message o)
        {
            var message = (NewCodeStorageMessage) o;
            var viewModel = sender as ViewModel;
            if (viewModel != null)
            {
                viewModel.Account.Key = message.Secret;
                if (message.Secret.Folder == null)
                {
                    message.Secret.Folder =
                        await  ApplicationData.Current.LocalFolder.CreateFolderAsync(viewModel.Account.Folder);
                }
                var folder = message.Secret.Folder;
                viewModel.Account.Key.Token= StorageApplicationPermissions.FutureAccessList.Add(folder);

                await viewModel.Account.Storage();
                await viewModel.Account.Key.Storage();
            }
        }

    }
}
