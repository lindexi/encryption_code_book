using System.Text;

namespace encryption_code_book.Model
{
    public class Account
    {
        public Account()
        {

        }

        public KeySecret Key
        {
            set;
            get;
        }

        public string Folder
        {
            set;
            get;
        } = "encryption";

        public string Patched
        {
            set;
            get;
        } = ".encry";

        public string FacitFile
        {
            set;
            get;
        } = "encryption.encry";

        public string EncryptionCodeNoteFolder
        {
            set;
            get;
        } = "EncryptionCode";

        public int ComfirmkeyLength
        {
            set;
            get;
        } = 1024;

        public Encoding Encod
        {
            set;
            get;
        } = Encoding.Unicode;
    }
}