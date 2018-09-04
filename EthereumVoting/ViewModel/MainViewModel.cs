using GalaSoft.MvvmLight;
using EthereumVoting.Model;
using System.Collections.Generic;
using EthereumVoting.Utilities;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using System.Collections;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;

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
        public IHelper GetHelper { get => helper;  }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IHelper _helper)
        {
            this.helper = _helper;
            //TestAsync();
            InitContractVotingAsync();
        }



        private void GetCandidates()
        {

        }

        private async Task TestAsync()
        {
            const string address = "0x12890d2cce102216644c59daE5baed380d84830c";
            const string address2 = "0x09211ef59f3c21b600c979c2122b2b32de9a86aa";
            const string pass2 = "123";

            const string pass = "password";

            const string abi = @"[{""constant"":false,""inputs"":[{""name"":""who"",""type"":""bytes32""}],""name"":""VoteFor"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""bytes32""}],""name"":""candidates"",""outputs"":[{""name"":""name"",""type"":""bytes32""},{""name"":""numVote"",""type"":""uint8""},{""name"":""isExist"",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""i"",""type"":""uint256""}],""name"":""GetAllCan"",""outputs"":[{""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""can"",""type"":""bytes32""}],""name"":""SetCandidate"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""GetVoterCount"",""outputs"":[{""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""listCan"",""outputs"":[{""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""address""}],""name"":""voters"",""outputs"":[{""name"":""availble"",""type"":""bool""},{""name"":""numVote"",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_voter"",""type"":""address""}],""name"":""SetVoter"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";


            const string byteCode = "60806040526002805460ff1916905534801561001a57600080fd5b506103aa8061002a6000396000f30060806040526004361061008d5763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416631925d44381146100925780631a0478d5146100ac578063649431e8146100e657806375ab6c2214610110578063805d9d4114610128578063841ada7814610153578063a3ec138d1461016b578063c6ff1274146101b6575b600080fd5b34801561009e57600080fd5b506100aa6004356101e4565b005b3480156100b857600080fd5b506100c4600435610242565b6040805193845260ff9092166020840152151582820152519081900360600190f35b3480156100f257600080fd5b506100fe600435610267565b60408051918252519081900360200190f35b34801561011c57600080fd5b506100aa60043561028a565b34801561013457600080fd5b5061013d6102fc565b6040805160ff9092168252519081900360200190f35b34801561015f57600080fd5b506100fe600435610305565b34801561017757600080fd5b5061019973ffffffffffffffffffffffffffffffffffffffff60043516610324565b60408051921515835260ff90911660208301528051918290030190f35b3480156101c257600080fd5b506100aa73ffffffffffffffffffffffffffffffffffffffff60043516610342565b60008181526020819052604090206001810154610100900460ff161561023e573360009081526003602090815260408083208590558483529082905290206001908101805460ff19811660ff918216909301169190911790555b5050565b6000602081905290815260409020805460019091015460ff8082169161010090041683565b600060018281548110151561027857fe5b90600052602060002001549050919050565b60008181526020819052604081208281556001908101805461010061ffff1990911617905580548082018255918190527fb10e2d527612073b26eecdfd717e6a320cf44b4afac2b0732d9fcbe2b7fa0cf6909101919091556002805460ff19811660ff91821690930116919091179055565b60025460ff1690565b600180548290811061031357fe5b600091825260209091200154905081565b60046020526000908152604090205460ff8082169161010090041682565b73ffffffffffffffffffffffffffffffffffffffff166000908152600460205260409020805461ff001960ff19909116600117166101001790555600a165627a7a72305820d6a0968aaee339a159d6570549056b7bdc869584586144090d5414a6cd1f3c7f0029";


            var web3 = new Web3("http://localhost:8545");

            var accounts = await web3.Personal.UnlockAccount.SendRequestAsync(address, pass, 60);
            //var receipt = await web3.Eth.DeployContract.EstimateGasAsync(abi, byteCode, address);
            var deloyContract = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(abi, byteCode, address, new HexBigInteger(900000));
            var contract = web3.Eth.GetContract(abi, deloyContract.ContractAddress);
            var funcSet = contract.GetFunction("SetCandidate");
            //var awaitSetCandiacate = await(contract.GetFunction("SetCandidate")).SendTransactionAndWaitForReceiptAsync(address, new HexBigInteger(900000), null, functionInput: "who");
            var a = await funcSet.SendTransactionAndWaitForReceiptAsync(address, gas: new HexBigInteger(900000), value: null, functionInput: "hello");
            //var count = await contract.GetFunction("GetVoterCount").CallAsync<int>();
            var c = await contract.GetFunction("listCan").CallAsync<string>(0);
        }

        private async Task InitContractVotingAsync()
        {
            try
            {
                GetHelper.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));
                const string address = "0x12890d2cce102216644c59daE5baed380d84830c";
                const string address2 = "0x09211ef59f3c21b600c979c2122b2b32de9a86aa";
                const string pass2 = "123";

                const string pass = "password";
                var r1 = await GetHelper.CheckUnlockAccountAsync(address, pass);
                //var r2 = await GetHelper.CheckUnlockAccountAsync(address2, pass2);
                
                var deployContract = await GetHelper.DeployContractAsync(ServiceLocator.Current.GetInstance<string>("abi"), ServiceLocator.Current.GetInstance<string>("bytecode"),
                    address);
               var ccc=  GetHelper.GetContract(ServiceLocator.Current.GetInstance<string>("abi"), deployContract.ContractAddress);
                for (int i = 0; i < 10; i++)
                {
                    await GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] { "who" + i });
                }

                ArrayList arrayList = new ArrayList();
                for (int l = 0; l < 10; l++)
                {
                    var result1 = await GetHelper.CallFunctionAsync<string>(address2, "listCan", new object[] { l });
                    arrayList.Add(result1);
                }
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