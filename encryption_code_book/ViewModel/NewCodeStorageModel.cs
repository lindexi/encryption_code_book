using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace encryption_code_book.ViewModel
{
    public class NewCodeStorageModel:NotifyProperty
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
            
        }

        public void NewEncryCodeFolder()
        {
            
        }

        public void NavigateEncryCodeStoragePage()
        {
            
        }
    }
}
