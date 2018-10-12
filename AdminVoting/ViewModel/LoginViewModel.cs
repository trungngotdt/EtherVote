using CommonLibraryUtilities;
using CommonLibraryUtilities.HelperMongo;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using CommonServiceLocator;
using MongoDB.Driver;
using System.Windows.Media;
using System.Windows.Controls;

namespace AdminVoting.ViewModel
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

        private ICommand commandBtnSubmitClick;

        public IHelper HelperUnti { get => helperUnti; set => helperUnti = value; }
        public IHelperMongo HelperMongoUnti { get => helperMongoUnti; set => helperMongoUnti = value; }
        public string Account { get => account; set => account = value; }
        public string Password { get => password; set => password = value; }
        public ICommand CommandBtnSubmitClick=> commandBtnSubmitClick = new RelayCommand(async () => {await SubmitClickAsync(); });

        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }
        public IRegisterParamaters RegisterParamaters { get => registerParamaters; set => registerParamaters = value; }

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
            HelperMongoUnti.GetClient("127.0.0.1", 27017, "user1", "pass1");
            HelperUnti.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));
            var database = HelperMongoUnti.GetDatabase("data1");
            getMongoCollection = HelperMongoUnti.GetMongoCollection();
            getMongoCollection.Init(database, "user", typeof(User));
            
        }
        
        async Task SubmitClickAsync()
        {
            Task<bool> task=HelperUnti.CheckUnlockAccountAsync(Account, Password);
            var ExpMenu =NavigationService.GetDescendantFromName(Application.Current.MainWindow, "ExpMenu") as Expander; 
            var builder = Builders<User>.Filter;
            var filter = builder.Eq("available", true) & builder.Eq("address", Account)&builder.Eq("role","admin");
            var user= getMongoCollection.GetData(filter);
            var checkAccount = user.Length > 0 && await task;
            if(checkAccount)
            {                
                RegisterParamaters.SetParamater("address", account);
                NavigationService.NavigateTo("Main");
                Account = null;
                Password = null;                
                ExpMenu.IsEnabled = true;
                ExpMenu.Visibility = Visibility.Visible;
            }
        }

    }
}
