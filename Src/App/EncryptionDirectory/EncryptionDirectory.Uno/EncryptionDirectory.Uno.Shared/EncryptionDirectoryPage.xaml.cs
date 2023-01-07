using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.IO;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EncryptionDirectory.Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EncryptionDirectoryPage : Page
    {
        public EncryptionDirectoryPage()
        {
            this.InitializeComponent();
            ViewModel = (EncryptionDirectoryViewModel) DataContext;
        }

        public EncryptionDirectoryViewModel ViewModel { get; }

        private void LogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            //textBox.SelectionStart = textBox.Text?.Length ?? 0;
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            button.IsEnabled = false;

            try
            {
                if (string.IsNullOrEmpty(ViewModel.SourcePath) || !Directory.Exists(ViewModel.SourcePath))
                {
                    ViewModel.Log($"没有找到源文件夹 {ViewModel.SourcePath}");
                    return;
                }

                if (string.IsNullOrEmpty(ViewModel.TargetPath))
                {
                    ViewModel.Log("加密输出文件夹不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(ViewModel.Key))
                {
                    ViewModel.Log("密码不能是空");
                    return;
                }

                var directoryEncryption = ViewModel.CreateDirectoryEncryption();

                if (DecryptionCheckBox.IsChecked is true)
                {
                    await directoryEncryption.DecryptDirectoryAsync(new DirectoryInfo(ViewModel.SourcePath));
                }
                else
                {
                    await directoryEncryption.UpdateAsync(new Progress<UpdateProgress>(progress =>
                    {
                        ViewModel.Log($"开始转换 {progress.CurrentFileName} {progress.RelativePath} {progress.CurrentFileIndex}/{progress.FileCount}");
                    }));

                    ViewModel.Log($"加密完成");
                }
            }
            finally
            {
                button.IsEnabled = true;
            }
        }
    }
}
