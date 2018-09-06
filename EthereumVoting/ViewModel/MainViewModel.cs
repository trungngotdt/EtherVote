using GalaSoft.MvvmLight;
using EthereumVoting.Model;
using System.Linq;
using System.Collections.Generic;
using EthereumVoting.Utilities;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using System.Collections;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System;

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
        const int barrerWait = 10;
        const int numFakeData = 20;
        private ObservableCollection<Candidate> candidates;

        private IHelper helper;


        private ICommand commandLoaded;
        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public ObservableCollection<Candidate> Candidates { get => candidates??new ObservableCollection<Candidate>(); set =>Set(ref candidates , value); }
        public IHelper GetHelper { get => helper; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(async () => { await InitContractVotingAsync(); await FakeDataAsync();await GetCandidatesAsync(); });

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper)
        {
            this.helper = _helper;
            //TestAsync();
            Candidates = new ObservableCollection<Candidate>();

        }

        private async Task FakeDataAsync()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                
                for (int i = 0; i < numFakeData; i++)
                {
                    tasks.Add(GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] { "who" + i }));
                    if(i%barrerWait==0&&i!=0||i==numFakeData-1)
                    {
                        await Task.WhenAll(tasks);
                        tasks = new List<Task>();
                    }
                }
                tasks = null;
            }
            catch (System.Exception)
            {
                throw;
            }
            
        }

        const string address = "0x12890d2cce102216644c59daE5baed380d84830c";
        const string address2 = "0x09211ef59f3c21b600c979c2122b2b32de9a86aa";

        const string pass2 = "123";

        const string pass = "password";

        private async Task GetCandidatesAsync()
        {
            //var r= await GetCandidateAsync();
            try
            {
                int checkCount = await GetCountAsync();
                List<Task<Candidate>> tasks = new List<Task<Candidate>>();
                for (int i = 0; i < numFakeData; i++)
                {                    
                    tasks.Add(Task.Factory.StartNew<Task<Candidate>>(async () =>
                    {
                       var result = await GetCandidateAsync(i);
                       return result;
                    }).Result);
                    if(i%barrerWait==0&&i!=0||i==numFakeData-1)
                    {
                        var resultTask = await Task.WhenAll<Candidate>(tasks);
                        Array.ForEach(resultTask, item => { Candidates.Add(item); });
                        //Candidates.AddRange(resultTask);
                        RaisePropertyChanged("Candidates");
                        resultTask = null;
                        tasks = new List<Task<Candidate>>();
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        private async Task<int> GetCountAsync()
        {
            int count = await GetHelper.CallFunctionAsync<int>(address, "GetVoterCount", null);
            return count;
        }

        public async Task<Candidate> GetCandidateAsync(int i)
        {
            var name = await GetHelper.CallFunctionAsync<string>(address, "listCan", new object[] { i });
            var task = await GetHelper.GetCallDeserializingToObjectAsync<Candidate>(address, "candidates", new object[] { name });
            return task;
        }


        private async Task InitContractVotingAsync()
        {
            try
            {
                GetHelper.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));

                var resultUnlock = await GetHelper.CheckUnlockAccountAsync(address, pass);
                //var r2 = await GetHelper.CheckUnlockAccountAsync(address2, pass2);

                var deployContract = await GetHelper.DeployContractAsync(ServiceLocator.Current.GetInstance<string>("abi"),
                    ServiceLocator.Current.GetInstance<string>("bytecode"),address);
                GetHelper.GetContract(ServiceLocator.Current.GetInstance<string>("abi"), deployContract.ContractAddress);
                /*
                for (int i = 0; i < 10; i++)
                {
                    await GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] { "who" + i });
                }

                ArrayList arrayList = new ArrayList();
                for (int l = 0; l < 10; l++)
                {
                    var result1 = await GetHelper.CallFunctionAsync<string>(address2, "listCan", new object[] { l });
                    arrayList.Add(result1);
                }*/
            }
            catch (System.Exception ex)
            {
                throw ex;
            }



        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}