using CommonLibraryUtilities;
using CommonLibraryUtilities.HelperMongo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdminVoting.ViewModel 
{
    public class DeployViewModel : ViewModelBase
    {
        private string abi;
        private string bytecode;

        private IHelper helper;
        private IHelperMongo helperMongo;
        private IRegisterParamaters registerParamaters;

        private ICommand commandBtnDeployClick;

        public IHelper GetHelper { get => helper; }
        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }
        public IRegisterParamaters RegisterParamaters { get => registerParamaters; set => registerParamaters = value; }
        public string Abi { get => abi; set => abi = value; }
        public string Bytecode { get => bytecode; set => bytecode = value; }


        public ICommand CommandBtnDeployClick => commandBtnDeployClick = new RelayCommand(async () => {await InitContractVotingAsync(); });

        string address;

        public DeployViewModel(IHelper _helper, IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters)
        {
            this.registerParamaters = _registerParamaters;
            this.helper = _helper;
            this.helperMongo = _helperMongo;
            Init();
        }

        

        private void Init()
        {
            address = registerParamaters.GetParamater("address").ToString();
        }

        private async Task InitContractVotingAsync()
        {
            try
            {
                var deployContract = await GetHelper.DeployContractAsync(Abi,
                    Bytecode, address);
                GetHelper.GetContract(Abi, deployContract.ContractAddress);
                await FakeDataAsync();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        private async Task FakeDataAsync()
        {
            try
            {
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < 3; i++)
                {
                    tasks.Add(GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] { "who" + i }));
                    
                        
                }
                await Task.WhenAll(tasks);
                tasks = null;
            }
            catch (System.Exception)
            {
                throw;
            }

        }
    }
}
