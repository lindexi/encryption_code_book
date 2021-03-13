namespace encryption_code_book.ViewModel
{
    public class KeyModel:ViewModelBase
    {
        public KeyModel()
        {
        }

        public void Comfirm()
        {
            //
            AccountGoverment.View.NacigateCode();
        }

        public override void OnNavigatedFrom(object obj)
        {
        }

        public override void OnNavigatedTo(object obj)
        {
        }

        public override void Receive(object source, Message message)
        {
        }
    }
}