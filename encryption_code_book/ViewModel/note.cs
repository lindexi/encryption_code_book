// lindexi
// 11:07

#region

using System;
using System.Collections.Generic;
using encryption_code_book.Model;

#endregion

namespace encryption_code_book.ViewModel
{
    public class note : viewModel
    {
        public note()
        {
            confim = false;
            text = "asidoch";
            _frist = false;
        }

        public void deserilization()
        {
            frame?.Navigate(typeof(key_page), this);

            //if (confim)
            //{

            //}
            //else
            //{
            //    frame.Navigate(typeof(key_page), this);
            //}
        }

        public new bool confim
        {
            set
            {
                base.confim = value;
            }
            get
            {
                return base.confim;
            }
        }

        public string text
        {
            set
            {
                _text_stack.Push(_text);
                _text = value;
                OnPropertyChanged();
            }
            get
            {
                return _text;
            }
        }

        private readonly Stack<string> _text_stack = new Stack<string>();
        private encryption_note _model = new encryption_note();
        private string _text;
        private string _key;

        public string key
        {
            set
            {
                _model.key = value;
            }
            get
            {
                return _model.key;
            }
        }

        public override bool first
        {
            get
            {
                return _model.first;
            }

            set
            {
                _model.first = value;
            }
        }
        private bool _first;

        public override bool confirm_password(string keystr)
        {
            if (first)
            {
                return confim = new_use(keystr);
            }
            confim = _model.confirm(keystr);
            if (!confim)
            {
                prompt = "密码错误，请重新输入";
            }
            return confim;
        }

        private bool new_use(string keystr)
        {
            if (_first)
            {
                if (string.Equals(_key, keystr))
                {
                    _model.new_use(keystr);
                    return true;
                }
                prompt = "密码错误，请重新输入";
                _first = false;
                return false;
            }
            else
            {
                _key = keystr;
                _first = true;
                prompt = "再次输入密码";
                return false;
            }
        }
        public void cancel()
        {
            try
            {
                _text = _text_stack.Pop();
            }
            catch (InvalidOperationException e)
            {
                reminder("没有修改" + e.Message + "\n");
            }
        }

        //public override bool confirm_password(string keystr)
        //{
        //    throw new NotImplementedException();
        //}
    }
}