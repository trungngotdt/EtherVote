using CommonLibrary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace AdminVoting.ViewModel
{
    public class ShellViewModel : ViewModelBase
    {
        private IFrameNavigationService _navigationService;

        private bool isEnabledExpMenu;
        private int visibilityExpMenu;

        private ICommand commandBtnDeployClickNavigation;
        private ICommand commandBtnUserClickNavigation;
        private ICommand commandLoaded;

        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(() => { NavigationService.NavigateTo("Login"); });

        public bool IsEnabledExpMenu { get => isEnabledExpMenu; set => isEnabledExpMenu = value; }

        /// <summary>
        /// Visible 0,Hidden 1,Collapsed 2
        /// </summary>
        public int VisibilityExpMenu { get => visibilityExpMenu; set => visibilityExpMenu = value; }
        public ICommand CommandBtnDeployClickNavigation => commandBtnDeployClickNavigation = new RelayCommand(() => { NavigationService.NavigateTo("Deploy"); });
        public ICommand CommandBtnUserClickNavigation =>commandBtnUserClickNavigation = new RelayCommand(() => { NavigationService.NavigateTo("Main"); });

        public ShellViewModel(IFrameNavigationService navigationService)
        {
            this._navigationService = navigationService;
        }
    
    }
}
