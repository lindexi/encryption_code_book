using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encryption_code_book.ViewModel
{
    public class fit : notify_property
    {
        public fit()
        {
            //todo
        }

        public void save_toggle()
        {
            save = !save;
            //todo
        }
        private bool _save;

        public bool save
        {
            set
            {
                _save = value;
                OnPropertyChanged();
            }
            get
            {
                return _save;
            }
        }

    }
}
