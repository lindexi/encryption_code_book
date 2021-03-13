using System;

namespace encryption_code_book.ViewModel
{
    public class Composite
    {
        public Type Message { get; set; }
        public string Key { get; set; }

        public virtual void Run(object sender, Message o)
        {
            
        }
    }
}