
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using CommonServiceLocator;
using CommonLibrary;
using CommonLibrary.HelperMongo;
using System.Windows.Controls;

namespace EthereumVoting.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string account;
        private string password;

        private IHelper helperUnti;
        private IHelperMongo helperMongoUnti;
        private IGetMongoCollection getMongoCollection;
        private IFrameNavigationService _navigationService;
        private IRegisterParamaters registerParamaters;
        private PropertiesOption option;

        private ICommand commandBtnSubmitClick;

        public IHelper HelperUnti { get => helperUnti; set => helperUnti = value; }
        public IHelperMongo HelperMongoUnti { get => helperMongoUnti; set => helperMongoUnti = value; }
        public string Account { get => account; set => account = value; }
        public string Password { get => password; set => password = value; }
        public ICommand CommandBtnSubmitClick=> commandBtnSubmitClick = new RelayCommand(async () => {await SubmitClickAsync(); });

        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }

        public PropertiesOption Option
        {
            get
            {
                if (option == null)
                {
                    return option = ServiceLocator.Current.GetInstance<PropertiesOption>("Option");
                }
                return option;
            }
        }

        public LoginViewModel(IHelper _helper, IHelperMongo _helperMongo, IFrameNavigationService navigationService,IRegisterParamaters _registerParamaters)
        {
            this.registerParamaters = _registerParamaters;
            this.helperUnti = _helper;
            this._navigationService = navigationService;
            this.helperMongoUnti = _helperMongo;
            Init();
        }

        void Init()
        {
            HelperMongoUnti.GetClient(Option.IpAddressMongoDefault, Option.PortMongoDefault, Option.NameUserMongoDefault, Option.PassUserMongoDefault);
            HelperUnti.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));
            var database = HelperMongoUnti.GetDatabase(Option.NameOfDBMongoDefault);
            getMongoCollection = HelperMongoUnti.GetMongoCollection();
            getMongoCollection.Init(database, "user", typeof(User));
            
        }
        
        async Task SubmitClickAsync()
        {
            Task<bool> task = HelperUnti.CheckUnlockAccountAsync(Account, Password);
            var builder = Builders<User>.Filter;
            var filter = builder.Eq("available", true) & builder.Eq("address", Account);
            var user= getMongoCollection.GetData(filter);
            var ExpMenu = NavigationService.GetDescendantFromName(Application.Current.MainWindow, "ExpMenu") as Expander;
            var checkAccount = user.Length > 0 && await task;
            if(checkAccount)
            {
                registerParamaters.SetParamater("address", account);
                NavigationService.NavigateTo("Main");
                Account = null;
                Password = null;
                ExpMenu.IsEnabled = true;
                ExpMenu.Visibility = Visibility.Visible;
            }
        }

    }
}
