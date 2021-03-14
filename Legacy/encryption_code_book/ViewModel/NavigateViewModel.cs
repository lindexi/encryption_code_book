using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Framework.ViewModel;

namespace encryption_code_book.ViewModel
{
    public abstract class NavigateViewModel : ViewModelBase, INavigato
    {
        public Frame Content
        {
            set;
            get;
        }

        public ViewModelBase this[string str]
        {
            get { return ViewModel.FirstOrDefault(temp => temp.Key == str)?.ViewModel; }
        }

        public List<ViewModelPage> ViewModel
        {
            set;
            get;
        }

        public async void Navigate(Type viewModel, object paramter)
        {
            _viewModel?.OnNavigatedFrom(null);
            ViewModelPage view = ViewModel.Find(temp => temp.Equals(viewModel));
            await view.Navigate(Content, paramter);
            view.ViewModel.Send += Receive;
            _viewModel = view.ViewModel;
        }
        //当前ViewModel
        private ViewModelBase _viewModel;
    }
}