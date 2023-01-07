using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace EncryptionDirectory.Wpf;
/// <summary>
/// EncryptionDirectoryUserControl.xaml 的交互逻辑
/// </summary>
public partial class EncryptionDirectoryUserControl : UserControl
{
    public EncryptionDirectoryUserControl()
    {
        InitializeComponent();
        ViewModel = (EncryptionDirectoryViewModel) DataContext;
    }

    public EncryptionDirectoryViewModel ViewModel { get; }

    private void LogTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        LogTextBox.ScrollToEnd();
    }

    private async void Button_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button) sender;
        button.IsEnabled = false;

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
        }

        button.IsEnabled = true;
    }
}

public class EncryptionDirectoryViewModel : INotifyPropertyChanged
{
    public DirectoryEncryption CreateDirectoryEncryption()
    {
        var directoryEncryption = new DirectoryEncryption(Key.Select(t => (int) t).ToArray(), new DirectoryInfo(SourcePath), new DirectoryInfo(TargetPath));
        return directoryEncryption;
    }

    public string Key
    {
        get => _key;
        set
        {
            if (value == _key) return;
            _key = value;
            OnPropertyChanged();
        }
    }

    public string SourcePath
    {
        get => _sourcePath;
        set
        {
            if (value == _sourcePath) return;
            _sourcePath = value;
            OnPropertyChanged();
        }
    }

    public string TargetPath
    {
        get => _targetPath;
        set
        {
            if (value == _targetPath) return;
            _targetPath = value;
            OnPropertyChanged();
        }
    }

    public string LogText
    {
        set
        {
            if (value == _logText) return;
            _logText = value;
            OnPropertyChanged();
        }
        get => _logText;
    }

    public void Log(string message)
    {
        LogText += DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss,fff") + " " + message + "\r\n";
    }

    private string _logText = string.Empty;
    private string _targetPath = string.Empty;
    private string _sourcePath = string.Empty;
    private string _key = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
