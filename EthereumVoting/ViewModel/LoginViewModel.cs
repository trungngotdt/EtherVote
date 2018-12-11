
using GalaSoft.MvvmLight;
using System;
using System.Collections;
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
using EthereumVoting.View;
using System.Windows.Media;

namespace EthereumVoting.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string account;
        private string password;
        private bool isOpenDialog;
        private object contentDialog;
        private bool isOpenSbNotify;
        private string messageSbNotify;

        private Brush accountBoderBrush;
        private Brush passwordBoderBrush;

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
        public ICommand CommandBtnSubmitClick => commandBtnSubmitClick = new RelayCommand(async () => { await SubmitClickAsync(); });

        public IFrameNavigationService NavigationService { get => _navigationService; set => _navigationService = value; }


        public bool IsOpenDialog { get => isOpenDialog; set { isOpenDialog = value; RaisePropertyChanged("IsOpenDialog"); } }
        public object ContentDialog { get => contentDialog; set { contentDialog = value; RaisePropertyChanged("ContentDialog"); } }


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

        public bool IsOpenSbNotify { get => isOpenSbNotify; set { isOpenSbNotify = value; RaisePropertyChanged("IsOpenSbNotify"); } }
        public string MessageSbNotify { get => messageSbNotify; set {messageSbNotify = value; RaisePropertyChanged("MessageSbNotify"); } }

        public Brush AccountBoderBrush { get => accountBoderBrush; set { accountBoderBrush = value;RaisePropertyChanged("AccountBoderBrush"); } }
        public Brush PasswordBoderBrush { get => passwordBoderBrush; set {passwordBoderBrush = value;RaisePropertyChanged("PasswordBoderBrush"); } }

        public LoginViewModel(IHelper _helper, IHelperMongo _helperMongo, IFrameNavigationService navigationService, IRegisterParamaters _registerParamaters)
        {
            this.registerParamaters = _registerParamaters;
            this.helperUnti = _helper;
            this._navigationService = navigationService;
            this.helperMongoUnti = _helperMongo;
            Init();
        }

        void Init()
        {
            ChangeBrush2Red(false);
            IsOpenDialog = true;
            ContentDialog = ServiceLocator.Current.GetInstance<ProgressDialogWindow>("Progress");
            Task taskProcess = Task.Factory.StartNew(() =>
            {
                HelperMongoUnti.GetClient(Option.IpAddressMongoDefault, Option.PortMongoDefault, Option.NameUserMongoDefault, Option.PassUserMongoDefault);
                HelperUnti.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));
                var database = HelperMongoUnti.GetDatabase(Option.NameOfDBMongoDefault);
                getMongoCollection = HelperMongoUnti.GetMongoCollection();
                getMongoCollection.Init(database, "user", typeof(User));
            }).ContinueWith(t => { IsOpenDialog = false; },TaskScheduler.FromCurrentSynchronizationContext());
        }

        async Task SubmitClickAsync()
        {
            IsOpenDialog = true;
            ContentDialog = ServiceLocator.Current.GetInstance<ProgressDialogWindow>("Progress");
            Task taskProcess = Task.Factory.StartNew(async () =>
            {
                try
                {
                    Task<bool> task = HelperUnti.CheckUnlockAccountAsync(Account, Password);
                    var builder = Builders<User>.Filter;
                    var filter = builder.Eq("available", true) & builder.Eq("address", Account);
                    var user = getMongoCollection.GetData(filter);
                    var checkAccount = user.Length > 0 && await task;
                    if (checkAccount)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var ExpMenu = NavigationService.GetDescendantFromName(Application.Current.MainWindow, "ExpMenu") as Expander;
                            registerParamaters.SetParamater("address", account);
                            registerParamaters.SetParamater("role", (user.GetValue(0) as User).Role);
                            NavigationService.NavigateTo("Main");
                            Account = null;
                            Password = null;
                            ExpMenu.IsEnabled = true;
                            ExpMenu.Visibility = Visibility.Visible;
                            OpenSnackBarNotify(false, "");
                        });
                    }
                    else
                    {
                        ChangeBrush2Red(true);
                        OpenSnackBarNotify(true,"Wrong account");
                    }
                    IsOpenDialog = false;
                }
                catch (Exception ex)
                {
                    ChangeBrush2Red(true);
                    IsOpenDialog = false;
                    OpenSnackBarNotify(true, ex.Message);
                    throw ex;
                }
                
            });
            await taskProcess;
            
        }

        private void ChangeBrush2Red(bool isChange)
        {
            AccountBoderBrush = isChange ? Brushes.Red : Brushes.Black;
            PasswordBoderBrush = isChange ? Brushes.Red : Brushes.Black;
        }

        private void OpenSnackBarNotify(bool isOpen,string message)
        {
            IsOpenSbNotify = isOpen;
            MessageSbNotify = message;
        }
    }
}
