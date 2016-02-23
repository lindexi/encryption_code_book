// lindexi
// 9:19

#region

using System;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using ViewModel;

#endregion

namespace encryption_code_book
{
    public class viewModel : notify_property
    {
        public viewModel()
        {
            mul = new mul_key_encryption();
            progress = Visibility.Collapsed;
            debug = Visibility.Collapsed;
            confim = false;
            cut_grid(1);
        }

        public string reminder
        {
            set
            {
                _reminder.Clear();
                _reminder.Append(value);
                OnPropertyChanged("reminder");
            }
            get
            {
                return _reminder.ToString();
            }
        }

        public Visibility register
        {
            set
            {
                _register = value;
                OnPropertyChanged("register");
            }
            get
            {
                return _register;
            }
        }

        public Visibility main_grid
        {
            set
            {
                _main_grid = value;
                OnPropertyChanged("main_grid");
            }
            get
            {
                return _main_grid;
            }
        }

        public Visibility help_grid
        {
            set
            {
                _help_grid = value;
                OnPropertyChanged("help_grid");
            }
            get
            {
                return _help_grid;
            }
        }

        public Visibility progress
        {
            set
            {
                _progress = value;
                OnPropertyChanged("progress");
                OnPropertyChanged("register_enable");
            }
            get
            {
                return _progress;
            }
        }

        public Visibility debug
        {
            set
            {
                _debug = value;
                OnPropertyChanged("debug");
            }
            get
            {
                return _debug;
            }
        }

        public bool register_enable
        {
            get
            {
                return progress == Visibility.Collapsed;
            }
        }

        public string help
        {
            set
            {
                _help = value;
                OnPropertyChanged("help");
            }
            get
            {
                return _help;
            }
        }

        /// <summary>
        ///     提示
        /// </summary>
        public string prompt
        {
            set
            {
                _prompt = value;
                OnPropertyChanged("prompt");
            }
            get
            {
                if (string.IsNullOrEmpty(_prompt))
                {
                    return fist_use() ? "请你输入一段不会忘记的可以是中文的密码" : "请输入密码";
                }
                return _prompt;
            }
        }

        public string file_key
        {
            set
            {
                _file_key = value;
                OnPropertyChanged("file_key");
            }
            get
            {
                return _file_key;
            }
        }

        /// <summary>
        ///     密码被修改
        /// </summary>
        public bool modify
        {
            set
            {
                _modify = value;
                OnPropertyChanged("modify");
            }
            get
            {
                return _modify;
            }
        }

        public bool confim
        {
            set
            {
                _confim = value;
            }
            get
            {
                return _confim;
            }
        }

        private readonly StringBuilder _reminder = new StringBuilder();
        private readonly mul_key_encryption mul;

        /// <summary>
        ///     确认密码
        /// </summary>
        private bool _confim;

        private Visibility _debug;
        private string _file_key;
        private string _help;
        private Visibility _help_grid;
        private Task _hold;
        private Visibility _main_grid;
        private bool _modify;
        private Visibility _progress;

        private string _prompt;
        private Visibility _register;

        public void ce()
        {
            //reminder = await mul.ce();
            //_confim = !_confim;
            //cut_grid(1);
            progress = Visibility.Visible;
        }

        public async void confirm_password(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                prompt = "请输入不为空的密码，可以是中文";
                return;
            }

            progress = Visibility.Visible;
            if (fist_use())
            {
                confim = true;
                mul.new_use(key, new_use_call_back);
            }
            else
            {
                if (await mul.confirm_password(key))
                {
                    confim = true;
                    file_key = await mul.jmz_decryption();
                    reminder = DateTime.Now + "文件解密成功" + "    可以使用组合键ctrl+s保存";
                }
                else
                {
                    confim = false;
                    prompt = "密码错误 重新输入";
                }
            }
            progress = Visibility.Collapsed;
            cut_grid(1);
        }

        public async void hold_asycn()
        {
            if (!confim)
            {
                return;
            }
            if (_hold != null)
            {
                await _hold;
            }
            if (!modify)
            {
                return;
            }
            modify = false;
            _hold = mul.override_entryption(file_key);
            reminder = DateTime.Now + "自动保存" + "    可以使用组合键ctrl+s保存";
        }

        public async Task hold()
        {
            if (confim)
            {
                await mul.override_entryption(file_key);
                modify = false;
                reminder = DateTime.Now + "保存";
            }
        }

        public void lock_grid()
        {
            confim = false;
            cut_grid(1);
            prompt = "请输入密码";
            reminder += " 锁住";
        }

        /// <summary>
        ///     判断第一次使用
        /// </summary>
        /// <returns></returns>
        private bool fist_use()
        {
            //本地没有出现 file address
            //漫游没有出现 file address
            return mul.first;
        }

        /// <summary>
        ///     切换界面
        /// </summary>
        /// <param name="grid">1注册 主2等 3帮助</param>
        private void cut_grid(int grid)
        {
            register = Visibility.Collapsed;
            main_grid = Visibility.Collapsed;
            help_grid = Visibility.Collapsed;
            switch (grid)
            {
                case 1:
                    if (confim)
                    {
                        main_grid = Visibility.Visible;
                        //if (mul.file != null)
                        //{
                        //    reminder = mul.file.Path;
                        //}
                    }
                    else
                    {
                        register = Visibility.Visible;
                    }
                    break;
                case 3:
                    help_grid = Visibility.Visible;
                    break;
                default:
                {
                    if (confim)
                    {
                        main_grid = Visibility.Visible;
                    }
                    else
                    {
                        register = Visibility.Visible;
                    }
                }
                    break;
            }
        }

        private void new_use_call_back(IAsyncResult ar)
        {
            reminder = DateTime.Now + "第一次使用，文件创建成功" + mul.file.Name + " " + "   文件会自动保存";
        }
    }
}