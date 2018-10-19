using CommonLibrary;
using CommonLibrary.HelperMongo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(() => { NavigationService.NavigateTo("Login"); });

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
