using System;

namespace encryption_code_book.ViewModel
{
    public class ViewModelAttribute : Attribute
    {
        public Type ViewModel { get; set; }
    }
}