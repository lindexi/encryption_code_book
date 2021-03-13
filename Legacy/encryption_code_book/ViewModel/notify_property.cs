// lindexi
// 10:30

#region

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

#endregion

namespace encryption_code_book.ViewModel
{
    /// <summary>
    /// 提供继承通知UI改变值
    /// </summary>
    public class notify_property : INotifyPropertyChanged
    {
        public notify_property()
        {
            reminder += str =>
            {
                if (string.IsNullOrEmpty(str))
                {
                    _reminder.Clear();
                }
                else
                {
                    _reminder.Append(str + "\n");
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<string> reminder;

        private readonly StringBuilder _reminder = new StringBuilder();

        public void UpdateProper<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            if (Equals(properValue, newValue))
            {
                return;
            }

            properValue = newValue;
            OnPropertyChanged(properName);
        }

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}