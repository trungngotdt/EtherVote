using CommonLibraryUtilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdminVoting.ViewModel
{
    public class ShellViewModel : ViewModelBase
    {
        private IFrameNavigationService _navigationService;

        private ICommand commandLoaded;
        private ICommand commandNavigeted;

        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(() => { NavigationService.NavigateTo("Login"); });


        public ShellViewModel(IFrameNavigationService navigationService)
        {
            this._navigationService = navigationService;
        }
    
    }
}
