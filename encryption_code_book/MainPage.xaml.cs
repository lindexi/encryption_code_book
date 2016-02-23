using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;
using encryption_code_book.ViewModel;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace encryption_code_book
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        viewModel view;
        
        public MainPage()
        {
            view = new viewModel();
            Application.Current.Suspending += Current_Suspending;
            this.Unloaded += MainPage_Unloaded;
            this.InitializeComponent();
            _ctrl = false;
            xkey.KeyDown += (sender, e)=>
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    confim(sender, null);
                }
            };
        }

        private void MainPage_Unloaded(object sender , RoutedEventArgs e)
        {
            view.hold_asycn();
        }

        private async void Current_Suspending(object sender , Windows.ApplicationModel.SuspendingEventArgs e)
        {
            view.file_key = xfile_key.Text;
            await view.hold();
            
            e.SuspendingOperation.Deadline.AddSeconds(5);
        }

        private void confim(object sender , RoutedEventArgs e)
        {
            //view.confirm_password(xkey.Password);
            //xkey.Password = string.Empty;
            view.confirm_password(xkey.Text);
            xkey.Text = string.Empty;
        }

        private void modify(object sender , TextChangedEventArgs e)
        {
            view.modify = true;
        }

        private void hold(object sender , RoutedEventArgs e)
        {            
            view.file_key = xfile_key.Text;
            view.hold_asycn();
        }

        private async void fastkey(object sender , KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                _ctrl = true;
            }
            else if (_ctrl && view.confim)
            {
                if (e.Key == Windows.System.VirtualKey.S)
                {
                    view.file_key = xfile_key.Text;
                    await view.hold();
                }
                else if (e.Key == Windows.System.VirtualKey.L)
                {
                    view.file_key = xfile_key.Text;
                    System.Threading.Tasks.Task t = view.hold();
                    view.lock_grid();
                }
            }
        }
        private bool _ctrl;        

        private void keyup(object sender , KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                _ctrl = false;
            }
        }
    }
}
