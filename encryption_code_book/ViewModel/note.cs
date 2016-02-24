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
        //private string_encryption encryption { set; get; } = new string_encryption();
        private string _text;
        private string _key;

        public string key
        {
            set
            {
                _key = value;
                _model.key = value;
            }
            get
            {
                return _model.key;
            }
        }

        public bool confim_password(string keystr)
        {
            return confim = _model.confirm(keystr);
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
    }
}