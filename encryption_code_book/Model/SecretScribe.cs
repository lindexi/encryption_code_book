namespace encryption_code_book.Model
{
    public class SecretScribe : KeySecret
    {
        public SecretScribe()
        {

        }

        private string _str;

        public string Str
        {
            set
            {
                _str = value;
                OnPropertyChanged();
            }
            get
            {
                return _str; 
            }
        }
    }
}