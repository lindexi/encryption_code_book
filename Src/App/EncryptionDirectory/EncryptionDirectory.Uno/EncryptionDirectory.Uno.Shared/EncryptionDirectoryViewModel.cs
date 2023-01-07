using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EncryptionDirectory.Uno;

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
        LogText = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss,fff") + " " + message + "\r\n";
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
