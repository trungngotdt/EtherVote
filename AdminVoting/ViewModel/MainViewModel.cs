using CommonLibraryUtilities;
using CommonLibraryUtilities.HelperMongo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace AdminVoting.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IHelper helper;
        private IHelperMongo helperMongo;
        private IRegisterParamaters registerParamaters;



        private ICommand commandLoaded;



        public ICommand CommandLoaded => commandLoaded = new RelayCommand();
        public IHelper Helper { get => helper; set => helper = value; }
        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }
        public IRegisterParamaters RegisterParamaters { get => registerParamaters; set => registerParamaters = value; }





        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper, IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters)
        {
            this.registerParamaters = _registerParamaters;
            this.helper = _helper;
            this.helperMongo = _helperMongo;
        }

        

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}