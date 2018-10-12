using CommonLibraryUtilities;
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


        private ICommand commandLoaded;
        private ICommand commandNavigeted;

        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(() => { NavigationService.NavigateTo("Login"); });

        public bool IsEnabledExpMenu { get => isEnabledExpMenu; set => isEnabledExpMenu = value; }

        /// <summary>
        /// Visible 0,Hidden 1,Collapsed 2
        /// </summary>
        public int VisibilityExpMenu { get => visibilityExpMenu; set => visibilityExpMenu = value; }

        public ShellViewModel(IFrameNavigationService navigationService)
        {
            this._navigationService = navigationService;
        }
    
    }
}
