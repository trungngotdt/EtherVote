﻿using GalaSoft.MvvmLight;
using CommonLibraryUtilities;
using CommonLibraryUtilities.HelperMongo;
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
        const int numFakeData = 3;
        private ObservableCollection<Candidate> candidates;

        private IHelper helper;
        private IHelperMongo helperMongo;
        private IRegisterParamaters registerParamaters;

        private ICommand commandLoaded;
        private ICommand commandBtnSubmitedClick;
        private ICommand commandChecked;

        public ObservableCollection<Candidate> Candidates { get => candidates ?? new ObservableCollection<Candidate>(); set => Set(ref candidates, value); }
        public IHelper GetHelper { get => helper; }
        public ICommand CommandLoaded => commandLoaded = new RelayCommand(async () => { await InitContractVotingAsync(); await FakeDataAsync(); await GetCandidatesAsync(); });

        public ICommand CommandBtnSubmitedClick => commandBtnSubmitedClick = new RelayCommand(async () => { await SubmitVotingAsync(); });

        public ICommand CommandChecked => commandChecked = new RelayCommand<string>((name) => { ToogleChecked(name); });

        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }

        string address;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper,IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters)
        {
            this.registerParamaters = _registerParamaters;
            this.helper = _helper;
            this.helperMongo = _helperMongo;
            Init();
        }

        private void Init()
        {
            HelperMongo.GetClient("127.0.0.1", 27017, "user1", "pass1");
            HelperMongo.GetDatabase("data1");
            address = registerParamaters.GetParamater("address").ToString();
            Candidates = new ObservableCollection<Candidate>();
        }

        private async Task FakeDataAsync()
        {
            try
            {
                /*
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < numFakeData; i++)
                {
                    tasks.Add(GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] { "who" + i }));
                    if (i % barrerWait == 0 && i != 0 || i == numFakeData - 1)
                    {
                        await Task.WhenAll(tasks);
                        tasks = new List<Task>();
                    }
                }
                tasks = null;*/
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        /*
        const string address2 = "0x09211ef59f3c21b600c979c2122b2b32de9a86aa";

        const string pass2 = "123";

        const string pass = "password";*/

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
                for (int i = 0; i < numFakeData; i++)
                {
                    tasks.Add(Task.Factory.StartNew<Task<Candidate>>(async () =>
                    {
                        var result = await GetACandidateAsync(i);
                        result.IsEnable = true;
                        return result;
                    }).Result);
                    /*
                    if (i % barrerWait == 0 && i != 0 || i == numFakeData - 1)
                    {
                        var resultTask = await Task.WhenAll<Candidate>(tasks);
                        arrayTemp.AddRange(resultTask);
                        resultTask = null;
                        tasks = new List<Task<Candidate>>();
                    }*/
                }
                var resultTask = await Task.WhenAll<Candidate>(tasks);
                arrayTemp.AddRange(resultTask);
                resultTask = null;
                tasks = new List<Task<Candidate>>();
                Candidates = new ObservableCollection<Candidate>(arrayTemp.OrderBy(i => i.Name));
                RaisePropertyChanged("Candidates");
                arrayTemp = null;
                tasks = null;
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

            var can = Candidates.AsParallel().FirstOrDefault(item => item.IsCheck);
            await GetHelper.SendTransactionFunctionAsync(address, "VoteFor", new object[] { can.Name });
            var task = await GetHelper.GetCallDeserializingToObjectAsync<Candidate>(address, "candidates", new object[] { can.Name });
            can.NumVote++;
            //NavigationService.Navigate();
        }

        private async Task InitContractVotingAsync()
        {
            try
            {
                //GetHelper.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));

                //var resultUnlock = await GetHelper.CheckUnlockAccountAsync(address, pass);
                /*
                var deployContract = await GetHelper.DeployContractAsync(ServiceLocator.Current.GetInstance<string>("abi"),
                    ServiceLocator.Current.GetInstance<string>("bytecode"), address);*/
                GetHelper.GetContract(ServiceLocator.Current.GetInstance<string>("abi"), "0x4b2dabefec292fe235263353162916ad09547864");
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