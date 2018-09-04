﻿using GalaSoft.MvvmLight;
using EthereumVoting.Model;
using System.Collections.Generic;
using EthereumVoting.Utilities;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using System.Collections;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

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
        private List<Candidate> candidates;

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

        public List<Candidate> Candidates { get => candidates; set => candidates = value; }
        public IHelper GetHelper { get => helper; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(async () => { await InitContractVotingAsync();await FakeDataAsync();GetCandidatesAsync(); });

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper)
        {
            this.helper = _helper;
            //TestAsync();
            

        }

        private async Task FakeDataAsync()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add( GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] { "who" + i }));
            }
            await Task.WhenAll(tasks);
        }

        const string address = "0x12890d2cce102216644c59daE5baed380d84830c";
        const string address2 = "0x09211ef59f3c21b600c979c2122b2b32de9a86aa";
        const string pass2 = "123";

        const string pass = "password";

        private async Task GetCandidatesAsync()
        {
            var r= await GetCandidateAsync();
            /*List<Task<Candidate>> tasks = new List<Task<Candidate>>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add( Task.Factory.StartNew<Candidate>(() =>
                {
                    var result= GetCandidateAsync(i).Result;
                    return  result;
                }));
                
            }
            var resultAll= await Task.WhenAll<Candidate>(tasks);*/
        }

        public async Task<Candidate[]> GetCandidateAsync()
        {
            List<Candidate> list = new List<Candidate>();
            for (int i = 0; i < 10; i++)
            {
                var name = await GetHelper.CallFunctionAsync<string>(address, "listCan", new object[] { i });
                var task = await GetHelper.GetCallDeserializingToObjectAsync<Candidate>(address, "candidates", new object[] { name });
                list.Add(task);
            }
            
            return list.ToArray();
        }


        private async Task InitContractVotingAsync()
        {
            try
            {
                GetHelper.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));

                var r1 = await GetHelper.CheckUnlockAccountAsync(address, pass);
                //var r2 = await GetHelper.CheckUnlockAccountAsync(address2, pass2);

                var deployContract = await GetHelper.DeployContractAsync(ServiceLocator.Current.GetInstance<string>("abi"), ServiceLocator.Current.GetInstance<string>("bytecode"),
                    address);
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