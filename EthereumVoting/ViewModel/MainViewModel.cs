using GalaSoft.MvvmLight;
using CommonLibrary;
using CommonLibrary.HelperMongo;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System;
using CommonServiceLocator;
using EthereumVoting.View;
using MongoDB.Driver;

namespace EthereumVoting.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Task<int> taskCountCandidates;



        const int barrerWait = 10;
        const int numFakeData = 3;
        private ObservableCollection<Candidate> candidates;
        private PropertiesOption option;
        private int countCandidates;
        private bool isOpenDialog;
        private object contentDialog;
        private bool isEnabledBtnSubmited;
        private bool isOpenSbNotify;
        private string messageSbNotify;

        private IHelper helper;
        private IHelperMongo helperMongo;
        private IGetMongoCollection getMongoCollection;
        private IRegisterParamaters registerParamaters;

        private ICommand commandLoaded;
        private ICommand commandBtnSubmitedClick;
        private ICommand commandChecked;

        public ObservableCollection<Candidate> Candidates { get => candidates ?? new ObservableCollection<Candidate>(); set => Set(ref candidates, value); }
        public IHelper GetHelper { get => helper; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(() =>
        {
            EffectProgress(()=> { CheckConditionFileConfig(async () => await LoadedAsync()); } );
        });

        public ICommand CommandBtnSubmitedClick => commandBtnSubmitedClick = new RelayCommand(() => 
        {
            EffectProgress(async () => { await SubmitVotingAsync(); });
        });
            //

        public ICommand CommandChecked => commandChecked = new RelayCommand<string>((name) => { ToogleChecked(name); });

        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }
        public PropertiesOption Option
        {
            get
            {               
                if(option==null)
                {
                    return option= ServiceLocator.Current.GetInstance<PropertiesOption>("Option");
                }
                return option;
            }
        }

        public bool IsOpenDialog { get => isOpenDialog; set { isOpenDialog = value; RaisePropertyChanged("IsOpenDialog"); } }
        public object ContentDialog { get => contentDialog; set { contentDialog = value; RaisePropertyChanged("ContentDialog"); } }

        public bool IsEnabledBtnSubmited { get => isEnabledBtnSubmited; set {isEnabledBtnSubmited = value; RaisePropertyChanged("IsEnabledBtnSubmited");} }

        public bool IsOpenSbNotify { get => isOpenSbNotify; set => isOpenSbNotify = value; }
        public string MessageSbNotify { get => messageSbNotify; set => messageSbNotify = value; }

        /// <summary>
        /// address of user in blockchain
        /// </summary>
        string address;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper,IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters)
        {
            this.registerParamaters = _registerParamaters;
            this.helper = _helper;
            this.helperMongo = _helperMongo;            
        }

        private async Task LoadedAsync()
        {
            Init();
            await InitContractVotingAsync();
            taskCountCandidates = GetHelper.CallFunctionAsync<int>(address, "GetVoterCount", new object[] {  });
            await GetCandidatesAsync();
            var votefor = await IsExistVoteFor();
            if (votefor.Trim().Count()!=0)
            {
                IsEnabledBtnSubmited = false;
                Candidates.AsParallel().ForAll(x => 
                {
                    x.IsCheck = x.Name.Equals(votefor);
                    x.IsEnable = false;
                });
            }
        }

        private void Init()
        {
            IsEnabledBtnSubmited = true;
            HelperMongo.GetClient(Option.IpAddressMongoDefault, Option.PortMongoDefault, Option.NameUserMongoDefault,Option.PassUserMongoDefault);
            var database= HelperMongo.GetDatabase(Option.NameOfDBMongoDefault);
            getMongoCollection = HelperMongo.GetMongoCollection();
            getMongoCollection.Init(database, "user", typeof(User));
            address = registerParamaters.GetParamater("address").ToString();
            Candidates = new ObservableCollection<Candidate>();
        }


        private void ToogleChecked(string name)
        {
            Candidates.AsParallel().ForAll(item => 
            {
                item.IsEnable = item.Name.Equals(name) || !item.IsEnable;
            });
            RaisePropertyChanged("Candidates");
        }

        private async Task GetCandidatesAsync()
        {
            try
            {
                var arrayTemp = new List<Candidate>();
                List<Task<Candidate>> tasks = new List<Task<Candidate>>();
                var count = await taskCountCandidates;
                for (int i = 0; i < count; i++)
                {
                    tasks.Add(Task.Factory.StartNew<Task<Candidate>>(async () =>
                    {
                        var result = await GetACandidateAsync(i);
                        result.IsEnable = true;
                        return result;
                    }).Result);                    
                }
                var resultTask = await Task.WhenAll<Candidate>(tasks);
                arrayTemp.AddRange(resultTask);
                
                Candidates = new ObservableCollection<Candidate>(arrayTemp.OrderBy(i => i.Name));
                RaisePropertyChanged("Candidates");
                arrayTemp = null;
                tasks = null;
                resultTask = null;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }


        public async Task<Candidate> GetACandidateAsync(int i)
        {
            var name = await GetHelper.CallFunctionAsync<string>(address, "listCan", new object[] { i });
            var task = await GetHelper.GetCallDeserializingToObjectAsync<Candidate>(address, "candidates", new object[] { name });
            return task;
        }

        private async Task SubmitVotingAsync()
        {
            try
            {
                var can = Candidates.AsParallel().FirstOrDefault(item => item.IsCheck);
                await GetHelper.SendTransactionFunctionAsync(address, "VoteFor", new object[] { can.Name });
                var previous = Builders<User>.Filter.Eq("address", address);
                var update = Builders<User>.Update.Set("VoteFor", can.Name);
                getMongoCollection.FindOneAndUpdateAsync(previous, update);
                //var task = await GetHelper.GetCallDeserializingToObjectAsync<Candidate>(address, "candidates", new object[] { can.Name });
                can.NumVote++;
                IsEnabledBtnSubmited = false;
                can.IsEnable = false;
                //NavigationService.Navigate();

            }
            catch (Exception ex)
            {
                OpenSnackBarNotify(true, ex.Message);
                throw ex;
            }
            
        }

        private async Task InitContractVotingAsync()
        {
            try
            {
                GetHelper.GetContract(ServiceLocator.Current.GetInstance<string>("abi"),Option.AddressContract );//"0x7ce7c3cd469c2cff44012bf5913675770b48a3a6");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private void CheckConditionFileConfig(Action method)
        {
            if (Option.Abi!=null&&Option.AddressContract != null && Option.ByteCode != null)
            {
                method();
            }
        }

        private Task< string> IsExistVoteFor()
        {
            return Task.Factory.StartNew<string>(() => 
            {
                var builder = Builders<User>.Filter;
                var filter = builder.Eq("available", true) & builder.Eq("address", address);
                var user = getMongoCollection.GetData(filter);

                return user.Cast<User>().ElementAt(0).VoteFor;

            });
            
        }

        private void EffectProgress(Action action)
        {
            IsOpenDialog = true;
            ContentDialog = ServiceLocator.Current.GetInstance<ProgressDialogWindow>("Progress");
            Task.Factory.StartNew(() =>
            {
                action();
                IsOpenDialog = false;
            }).ContinueWith(t => {  }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OpenSnackBarNotify(bool isOpen, string message)
        {
            IsOpenSbNotify = isOpen;
            MessageSbNotify = message;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}