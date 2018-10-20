using CommonLibrary;
using CommonLibrary.HelperMongo;
using CommonServiceLocator;
using EthereumVoting.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EthereumVoting.ViewModel
{
    public class ShellViewModel: ViewModelBase
    {
        private IFrameNavigationService _navigationService;

        private ICommand commandLoaded;
        private ICommand commandNavigeted;
        private ICommand commandBtnConfigClickNavigation;
        private ICommand commandBtnUserClickNavigation;
        private bool isOpenDialog;
        private object contentDialog;


        public bool IsOpenDialog { get => isOpenDialog; set { isOpenDialog = value; RaisePropertyChanged("IsOpenDialog"); } }
        public object ContentDialog { get => contentDialog; set { contentDialog = value; RaisePropertyChanged("ContentDialog"); } }
        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(() => 
        {
            IsOpenDialog = true;
            ContentDialog = ServiceLocator.Current.GetInstance<ProgressDialogWindow>("Progress");
            Task.Factory.StartNew(() => 
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NavigationService.NavigateTo("Login");
                });
                IsOpenDialog = false;
            });            
        });

        public ICommand CommandNavigeted => commandNavigeted = new RelayCommand<Frame>(x => 
        {
            //x.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
        });

        public ICommand CommandBtnConfigClickNavigation => commandBtnConfigClickNavigation = new RelayCommand(() => { NavigationService.NavigateTo("Config"); });
        public ICommand CommandBtnUserClickNavigation => commandBtnUserClickNavigation = new RelayCommand(() => { NavigationService.NavigateTo("Main"); });

        public ShellViewModel(IFrameNavigationService navigationService)
        {
            this._navigationService = navigationService;
        }

        
    }
}
