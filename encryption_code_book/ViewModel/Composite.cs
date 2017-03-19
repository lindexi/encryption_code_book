using System;
using encryption_code_book.ViewModel;

namespace Framework.ViewModel
{
    public class Composite
    {
        public Type Message { get; set; }
        public string Key { get; set; }

        public virtual void Run(ViewModelBase source, Message o)
        {
            
        }
    }
}