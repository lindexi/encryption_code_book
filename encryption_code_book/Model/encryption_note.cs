using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encryption_code_book.Model
{
    public class encryption_note
    {
        public encryption_note()
        {

        }

        private string _encryption_text;
        private string _encryption_key;
        public bool read_encryption { set; get; }

        private bool _read_key;
        private Windows.Storage.StorageFile _file;
        /// <summary>
        /// 保存
        /// </summary>
        public void storage()
        {
            string str = encryption_key + encryption_text;

        }

        private void read()
        {
            
        }

        public string encryption_key
        {
            set
            {
                _encryption_key = value;
            }
            get
            {
                return _encryption_key;
            }
        }

        public string encryption_text
        {
            set
            {
                _encryption_text = value;
            }
            get
            {
                return _encryption_text;
            }
        }
        /// <summary>
        /// 密码保存的文件读取
        /// </summary>
        public bool read_key
        {
            get
            {
                return _read_key;
            }
            set
            {
                _read_key = value;
            }
        }
    }
}
