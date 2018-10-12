using CommonLibraryUtilities;
using CommonLibraryUtilities.HelperMongo;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminVoting.ViewModel 
{
    public class DeployViewModel : ViewModelBase
    {
        private string abi;
        private string bytecode;

        private IHelper helper;
        private IHelperMongo helperMongo;
        private IRegisterParamaters registerParamaters;

        public IHelper GetHelper { get => helper; }
        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }
        public IRegisterParamaters RegisterParamaters { get => registerParamaters; set => registerParamaters = value; }
        public string Abi { get => abi; set => abi = value; }
        public string Bytecode { get => bytecode; set => bytecode = value; }


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
            //throw new NotImplementedException();
            InitContractVotingAsync();
        }

        private async Task InitContractVotingAsync()
        {
            try
            {
                var deployContract = await GetHelper.DeployContractAsync(Abi,
                    Bytecode, address);
                GetHelper.GetContract(Abi, deployContract.ContractAddress);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
