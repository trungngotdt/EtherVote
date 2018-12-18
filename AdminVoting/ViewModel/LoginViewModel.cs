using CommonLibrary;
using CommonLibrary.HelperMongo;
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
using AdminVoting.View;

namespace AdminVoting.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private bool isOpenDialog;
        private object contentDialog;
        private bool isOpenSbNotify;
        private string messageSbNotify;
        
        private PropertiesOption option;

        public ICommand CommandBtnSubmitClick { get; set; }

        private Brush accountBoderBrush;
        private Brush passwordBoderBrush;



        private IGetMongoCollection HelpGetMongoCollection { get; set; }
        private IHelper HelperUnti { get ; set ; }
        private IHelperMongo HelperMongoUnti { get; set; }
        public string Account { get ; set ; }
        public string Password { get ; set ; }
        
        
        public IFrameNavigationService NavigationService { get ; set; }
        public IRegisterParamaters RegisterParamaters { get;set; }

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
        public string MessageSbNotify { get => messageSbNotify; set { messageSbNotify = value; RaisePropertyChanged("MessageSbNotify"); } }

        public Brush AccountBoderBrush { get => accountBoderBrush; set { accountBoderBrush = value; RaisePropertyChanged("AccountBoderBrush"); } }
        public Brush PasswordBoderBrush { get => passwordBoderBrush; set { passwordBoderBrush = value; RaisePropertyChanged("PasswordBoderBrush"); } }




        public LoginViewModel( IHelper _helper, IHelperMongo _helperMongo, IFrameNavigationService navigationService,IRegisterParamaters _registerParamaters)
        {
            
            this.RegisterParamaters = _registerParamaters;
            this.HelperUnti = _helper;
            this.NavigationService = navigationService;
            this.HelperMongoUnti = _helperMongo;
            CheckException(() => {Init(); });
            
        }        

        void Init()
        {

            ChangeBrush2Red(false);
            OpenDialog(true);
            Task.Factory.StartNew(() =>
            {
                HelperMongoUnti.GetClient(Option.IpAddressMongoDefault, Option.PortMongoDefault, Option.NameUserMongoDefault, Option.PassUserMongoDefault);
                HelperUnti.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));
                var database = HelperMongoUnti.GetDatabase(Option.NameOfDBMongoDefault);
                HelpGetMongoCollection = HelperMongoUnti.GetMongoCollection();
                HelpGetMongoCollection.Init(database, "user", typeof(User));
            }).ContinueWith(t => { OpenDialog(false); }, TaskScheduler.FromCurrentSynchronizationContext());
            InitCommand();
            /*
             ChangeBrush2Red(false);
            HelperMongoUnti.GetClient("127.0.0.1", 27017, "user1", "pass1");
            HelperUnti.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));
            var database = HelperMongoUnti.GetDatabase("data1");
            HelpGetMongoCollection = HelperMongoUnti.GetMongoCollection();
            HelpGetMongoCollection.Init(database, "user", typeof(User));
             */


        }

        void InitCommand()
        {
            CommandBtnSubmitClick = new RelayCommand(() =>
            {
                CheckException(async () => { await SubmitClickAsync(); });
            });
        }


        async Task SubmitClickAsync()
        {
            OpenDialog(true);
            Task taskProcess = Task.Factory.StartNew(async () =>
            {
                try
                {
                    Task<bool> task = HelperUnti.CheckUnlockAccountAsync(Account, Password);
                    var builder = Builders<User>.Filter;
                    var filter = builder.Eq("available", true) & builder.Eq("address", Account) & builder.Eq("role", "admin"); ;
                    var user = HelpGetMongoCollection.GetData(filter);
                    var checkAccount = user.Length > 0 && await task;
                    if (checkAccount)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var ExpMenu = NavigationService.GetDescendantFromName(Application.Current.MainWindow, "ExpMenu") as Expander;
                            RegisterParamaters.SetParamater("address", Account);
                            RegisterParamaters.SetParamater("role", (user.GetValue(0) as User).Role);
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
                        OpenSnackBarNotify(true, "Wrong account");
                    }
                    IsOpenDialog = false;
                }
                catch (Exception ex)
                {
                    ChangeBrush2Red(true);
                    OpenDialog(false);
                    OpenSnackBarNotify(true, ex.Message);
                    //throw ex;
                }

            });
            await taskProcess;

        }

        /*
        async Task SubmitClickAsync()
        {
            IsOpenDialog = true;
            ContentDialog = ServiceLocator.Current.GetInstance<ProgressDialogWindow>("Progress");
            
            var ExpMenu =NavigationService.GetDescendantFromName(Application.Current.MainWindow, "ExpMenu") as Expander;
            await Task.Factory.StartNew(async () =>
             {
                 Task<bool> task = HelperUnti.CheckUnlockAccountAsync(Account, Password);

                 var builder = Builders<User>.Filter;
                 var filter = builder.Eq("available", true) & builder.Eq("address", Account) & builder.Eq("role", "admin");
                 var user = HelpGetMongoCollection.GetData(filter);
                 var checkAccount = user.Length > 0 && await task;
                 if (checkAccount)
                 {
                     Application.Current.Dispatcher.Invoke(() =>
                     {
                         ExpMenu.IsEnabled = true;
                         ExpMenu.Visibility = Visibility.Visible;
                         NavigationService.NavigateTo("Main");
                     });
                     RegisterParamaters.SetParamater("address", account);
                     Account = null;
                     Password = null;
                     IsOpenDialog = false;
                 }
                 else
                 {
                     OpenSnackBarNotify(true, "Wrong Account");
                 }
             });            
        }
        */


        private void ChangeBrush2Red(bool isChange)
        {
            AccountBoderBrush = isChange ? Brushes.Red : Brushes.Black;
            PasswordBoderBrush = isChange ? Brushes.Red : Brushes.Black;
        }

        private void OpenDialog(bool isOpen)
        {
            IsOpenDialog = isOpen;
            ContentDialog = isOpen?ServiceLocator.Current.GetInstance<ProgressDialogWindow>("Progress"):null;
        }

        private void OpenSnackBarNotify(bool isOpen, string message)
        {
            IsOpenSbNotify = isOpen;
            MessageSbNotify = message;
        }

        private void CheckException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                OpenSnackBarNotify(true, ex.Message);
                throw;
            }
        }

    }
}
