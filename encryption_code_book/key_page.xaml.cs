using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using encryption_code_book.ViewModel;

namespace encryption_code_book
{
    public partial class key_page : Page
    {
        private ViewModel.viewModel view;
        public key_page()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            view = e.Parameter as viewModel;//?? new viewModel();
        }

        private void confirm(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            view.confirm_password(view.key);
            view.key = "";
        }
    }
}
