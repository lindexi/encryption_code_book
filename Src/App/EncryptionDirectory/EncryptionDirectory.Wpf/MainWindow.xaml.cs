using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using Path = System.IO.Path;

namespace EncryptionDirectory.Wpf;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OpenMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog()
        {
            Filter = "加密文件夹配置文件(*.encd)|*.encd"
        };
        openFileDialog.ShowDialog(this);

        var file = openFileDialog.FileName;
        if (!string.IsNullOrEmpty(file) && File.Exists(file))
        {
            await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            var encryptionDirectoryConfigurationSaveInfo = await JsonSerializer.DeserializeAsync<EncryptionDirectoryConfigurationSaveInfo>(fileStream);
            if (encryptionDirectoryConfigurationSaveInfo != null)
            {
                EncryptionDirectoryUserControl.ViewModel.SourcePath =
                    encryptionDirectoryConfigurationSaveInfo.SourcePath;
                EncryptionDirectoryUserControl.ViewModel.TargetPath =
                    encryptionDirectoryConfigurationSaveInfo.TargetPath;
            }
        }
    }

    private async void SaveMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog()
        {
            Filter = "加密文件夹配置文件(*.encd)|*.encd"
        };
        saveFileDialog.ShowDialog(this);
        var file = saveFileDialog.FileName;
        if (!string.IsNullOrEmpty(file))
        {
            var encryptionDirectoryConfigurationSaveInfo = new EncryptionDirectoryConfigurationSaveInfo(EncryptionDirectoryUserControl.ViewModel.SourcePath, EncryptionDirectoryUserControl.ViewModel.TargetPath);

            await using var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(fileStream, encryptionDirectoryConfigurationSaveInfo,new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
    }

    private void QuitMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("可以将文件夹加密以用来同步到网盘。本软件完全开源免费，请参阅 https://github.com/lindexi/encryption_code_book", "加密文件夹工具");
    }

    private const string FileExtension = ".encd";

    record EncryptionDirectoryConfigurationSaveInfo(string SourcePath, string TargetPath)
    {
    }
}
