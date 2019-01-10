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
        private int countCheckedCandidates = 2;

        private Task<int> taskCountCandidates;


        private ObservableCollection<Candidate> candidates;
        private PropertiesOption option;

        private bool isOpenDialog;
        private object contentDialog;
        private bool isEnabledBtnSubmited;
        private bool isOpenSbNotify;
        private string messageSbNotify;
        public int NOVoteWidth { get; set; }
        private List<string> candidatesIsCheck;

        private IHelper helper;
        private IGetMongoCollection HelpGetMongoCollection { get; set; }
        private IRegisterParamaters RegisterParamaters { get; set; }


        public ObservableCollection<Candidate> Candidates { get => candidates ?? new ObservableCollection<Candidate>(); set => Set(ref candidates, value); }
        public IHelper GetHelper { get => helper; }
        public ICommand CommandLoaded { get; set; }

        public ICommand CommandBtnSubmitedClick { get; set; }


        public ICommand CommandChecked { get; set; }

        public IHelperMongo HelperMongo { get; set; }
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

        public bool IsOpenDialog { get => isOpenDialog; set { isOpenDialog = value; RaisePropertyChanged("IsOpenDialog"); } }
        public object ContentDialog { get => contentDialog; set { contentDialog = value; RaisePropertyChanged("ContentDialog"); } }

        public bool IsEnabledBtnSubmited { get => isEnabledBtnSubmited; set { isEnabledBtnSubmited = value; RaisePropertyChanged("IsEnabledBtnSubmited"); } }

        public bool IsOpenSbNotify { get => isOpenSbNotify; set { isOpenSbNotify = value; RaisePropertyChanged("IsOpenSbNotify"); } }
        public string MessageSbNotify { get => messageSbNotify; set { messageSbNotify = value; RaisePropertyChanged("MessageSbNotify"); } }

        /// <summary>
        /// address of user in blockchain
        /// </summary>
        string address;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper, IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters)
        {
            this.RegisterParamaters = _registerParamaters;
            this.helper = _helper;
            this.HelperMongo = _helperMongo;
            candidatesIsCheck = new List<string>();
            InitCommand();
            CommandLoaded = new RelayCommand(() =>
            {
                EffectProgress(() =>
                {
                    CheckConditionFileConfig(async () =>
                    {
                        try
                        {
                            
                            await LoadedAsync();
                        }
                        catch (Exception ex)
                        {
                            OpenDialog(false);
                            OpenSnackBarNotify(true, ex.Message);
                        }
                    });
                });
            });
        }

        private async Task LoadedAsync()
        {
            OpenDialog(true);
            Init();
            await InitContractVotingAsync();
            taskCountCandidates = GetHelper.CallFunctionAsync<int>(address, "GetVoterCount", new object[] { });
            Task taskGetCandidate = GetCandidatesAsync();
            var role = RegisterParamaters.GetParamater("role");
            NOVoteWidth = role.Equals("admin") ? 200 : 0;
            RaisePropertyChanged("NOVoteWidth");
            //InitCommand();
            await taskGetCandidate;
            if (role.Equals("admin"))
            {
                Candidates.AsParallel().ForAll(x => {x.IsEnable = false;});                
            }
            else
            {
                var votefor = await IsExistVoteFor();
                if (!String.IsNullOrEmpty(votefor))
                {
                    var candidates = votefor.Split(' ');
                    IsEnabledBtnSubmited = false;
                    Candidates.AsParallel().ForAll(x =>
                    {
                        x.IsCheck = candidates.Contains(x.Name);// x.Name.Equals(votefor);
                        x.IsEnable = false;
                    });
                }
            }
            OpenDialog(false);
            RaisePropertyChanged("Candidates");
        }

        private void Init()
        {
            IsEnabledBtnSubmited = true;
            HelperMongo.GetClient(Option.IpAddressMongoDefault, Option.PortMongoDefault, Option.NameUserMongoDefault, Option.PassUserMongoDefault);
            var database = HelperMongo.GetDatabase(Option.NameOfDBMongoDefault);
            HelpGetMongoCollection = HelperMongo.GetMongoCollection();
            HelpGetMongoCollection.Init(database, "user", typeof(User));
            address = RegisterParamaters.GetParamater("address").ToString();
            Candidates = new ObservableCollection<Candidate>();
        }

        private void InitCommand()
        {
            CommandChecked = new RelayCommand<string>((name) => { ToogleChecked(name); });
            CommandBtnSubmitedClick = new RelayCommand(() =>
            {
                EffectProgress(async () =>
                {
                    try
                    {
                        OpenDialog(true);
                        await SubmitVotingAsync();
                    }
                    catch (Exception ex)
                    {
                        OpenDialog(false);
                        OpenSnackBarNotify(true, ex.Message);
                    }
                });
            });
        }

        private void ToogleChecked(string name)
        {
            var itemIsAlReadyCheck = candidatesIsCheck.AsParallel().Contains(name);
            if (itemIsAlReadyCheck)
            {
                Candidates.AsParallel().First(item => item.Name.Equals(name)).IsCheck = false;
                candidatesIsCheck.Remove(name);
                if (countCheckedCandidates == 0)
                {
                    Candidates.AsParallel().ForAll(item =>
                    {
                        item.IsEnable = true;
                    });
                }
                countCheckedCandidates++;
                return;
            }
            candidatesIsCheck.Add(name);
            if (countCheckedCandidates == 1)
            {
                Candidates.AsParallel().ForAll(item =>
                {
                    item.IsEnable = candidatesIsCheck.AsParallel().Contains(item.Name) || !item.IsEnable;
                });
                RaisePropertyChanged("Candidates");
                countCheckedCandidates--;
                return;
            }
            countCheckedCandidates--;
        }

        private async Task GetCandidatesAsync()
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

        public async Task<Candidate> GetACandidateAsync(int i)
        {
            var name = await GetHelper.CallFunctionAsync<string>(address, "listCan", new object[] { i });
            var task = await GetHelper.GetCallDeserializingToObjectAsync<Candidate>(address, "candidates", new object[] { name });
            return task;
        }

        private async Task SubmitVotingAsync()
        {
            if (countCheckedCandidates != 0)
            {
                throw (new Exception("You have " + countCheckedCandidates + " candidates for voting"));
            }
            foreach (var item in candidatesIsCheck)
            {
                var voting = GetHelper.SendTransactionFunctionAsync(address, "VoteFor", new object[] { item });
                var can = Candidates.AsParallel().FirstOrDefault(i => i.Name.Equals(item));
                can.NumVote++;
                can.IsEnable = false;
                await voting;
            }
            var previous = Builders<User>.Filter.Eq("address", address);
            var update = Builders<User>.Update.Set("VoteFor", String.Join(" ", candidatesIsCheck.ToArray()));
            HelpGetMongoCollection.FindOneAndUpdateAsync(previous, update);
            IsEnabledBtnSubmited = false;
        }

        private async Task InitContractVotingAsync()
        {
            GetHelper.GetContract(ServiceLocator.Current.GetInstance<string>("abi"), Option.AddressContract);//"0x7ce7c3cd469c2cff44012bf5913675770b48a3a6");
        }

        private void CheckConditionFileConfig(Action method)
        {
            if (Option.Abi != null && Option.AddressContract != null && Option.ByteCode != null)
            {
                method();
            }
        }

        private Task<string> IsExistVoteFor()
        {
            return Task.Factory.StartNew<string>(() =>
            {
                var builder = Builders<User>.Filter;
                var filter = builder.Eq("available", true) & builder.Eq("address", address);
                var user = HelpGetMongoCollection.GetData(filter);

                return user.Length != 0 ? user.Cast<User>().FirstOrDefault().VoteFor : null;
            });

        }

        private void EffectProgress(Action action)
        {
            try
            {
                OpenDialog(true);
                Task.Factory.StartNew(() =>
                {
                    action();
                    IsOpenDialog = false;
                }).ContinueWith(t => { }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (Exception ex)
            {
                OpenSnackBarNotify(true, ex.Message);
                OpenDialog(false);
                //throw;
            }

        }


        private void OpenDialog(bool isOpen)
        {
            IsOpenDialog = isOpen;
            ContentDialog = isOpen ? ServiceLocator.Current.GetInstance<ProgressDialogWindow>("Progress") : null;
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