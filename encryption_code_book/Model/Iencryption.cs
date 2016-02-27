namespace encryption_code_book.Model
{
    public interface Iencryption
    {
        bool confirm(string keystr, string key);
        string decryption(string str, string key);
        string encryption(string str, string key);
        string n_md5(string key);
    }
}