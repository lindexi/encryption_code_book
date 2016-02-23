// lindexi
// 10:23

#region

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using encryption_code_book.ViewModel;

#endregion

namespace encryption_code_book
{
    public partial class note_page : Page
    {
        public note_page()
        {
            //view=new note();
            _ctrl = false;
            InitializeComponent();
        }

        private note view;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            view = e.Parameter as note ?? new note();
        }

        private void modify(object sender, TextChangedEventArgs e)
        {

        }

        private void hold(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void fastkey(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                _ctrl = true;
            }
            //else if (_ctrl && view.confim)
            //{
            //    if (e.Key == Windows.System.VirtualKey.S)
            //    {
            //        view.file_key = xfile_key.Text;
            //        await view.hold();
            //    }
            //    else if (e.Key == Windows.System.VirtualKey.L)
            //    {
            //        view.file_key = xfile_key.Text;
            //        System.Threading.Tasks.Task t = view.hold();
            //        view.lock_grid();
            //    }
            //}
        }

        private void keyup(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                _ctrl = false;
            }
        }

        private bool _ctrl;
    }
}