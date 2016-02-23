// lindexi
// 10:23

#region

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    }
}