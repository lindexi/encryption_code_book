using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using encryption_code_book.Model;
using encryption_code_book.View;

namespace encryption_code_book.ViewModel
{
    public class AccountGoverment
    {
        public AccountGoverment()
        {
            Account = new Account();
            Read();
        }

        public static AccountGoverment View
        {
            set
            {
                _accountGoverment = value;
            }
            get
            {
                return _accountGoverment ?? (_accountGoverment = new AccountGoverment());
            }
        }

        //密码输入一次
        //如果还有密码输入，打开文件不匹配的密码
        public Account Account
        {
            set;
            get;
        }

        public Frame Frame
        {
            set;
            get;
        }


        public KeySecret Key
        {
            set;
            get;
        }

        //public SecretCode EncryCodeStorage
        //{
        //    set;
        //    get;
        //}

        public List<SecretScribe> EncryCodeStorage
        {
            set;
            get;
        }

        private async void Read()
        {
            //文件夹
            Key = new KeySecret();
            Account.Key = Key;
            try
            {
                StorageFolder folder = null;
                StorageFile file;
                try
                {
                    folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(
                        Account.Folder);
                    file = await folder.GetFileAsync(
                        Account.FacitFile);
                    await Key.Read(file);
                }
                catch (FileNotFoundException)
                {
                    if (folder == null)
                    {
                        folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(
                            Account.Folder);
                    }
                    file = await folder.CreateFileAsync(Account.FacitFile);
                    Key.File = file;
                }
            }
            catch
            {

            }
        }

        public void NavigateKey()
        {
            Frame.Navigate(typeof(KeyPage));
        }

        public void NacigateCode()
        {
            Frame.Navigate(typeof(CodePage));
        }

        private static AccountGoverment _accountGoverment;
    }
}