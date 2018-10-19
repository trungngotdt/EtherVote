using CommonLibrary;
using CommonLibrary.HelperMongo;
using CommonServiceLocator;
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
        private string fileAddress;

        private PropertiesOption option;

        private List<string> candidates;

        private IHelper helper;
        private IHelperMongo helperMongo;
        private IRegisterParamaters registerParamaters;
        private IWorkExcel excel;
        private IWorkJson json;

        private ICommand commandBtnDeployClick;
        private ICommand commandBtnOpenFile;


        public IHelper GetHelper { get => helper; }
        public IHelperMongo HelperMongo { get => helperMongo; set => helperMongo = value; }
        public IRegisterParamaters RegisterParamaters { get => registerParamaters; set => registerParamaters = value; }
        public string Abi { get => abi; set => abi = value; }
        public string Bytecode { get => bytecode; set => bytecode = value; }


        public ICommand CommandBtnDeployClick => commandBtnDeployClick = new RelayCommand(async () => {await InitContractVotingAsync(); });

        public ICommand CommandBtnOpenFile => commandBtnOpenFile = new RelayCommand(() => { OpenLoadFile(); });

        public List<string> Candidates { get => candidates; set => candidates = value; }
        public string FileAddress { get => fileAddress; set { fileAddress = value; RaisePropertyChanged("FileAddress"); } }
        public IWorkExcel Excel { get => excel; set => excel = value; }
        public IWorkJson Json { get => json; set => json = value; }

        string address;
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


        public DeployViewModel(IHelper _helper, IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters,IWorkExcel workExcel,IWorkJson workJson)
        {
            this.registerParamaters = _registerParamaters;
            this.helper = _helper;
            this.helperMongo = _helperMongo;
            this.excel = workExcel;
            this.json = workJson;
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
                Task<List<object>> list = Task.Factory.StartNew<List<object>>(() => Excel.GetData(FileAddress));
                var deployContract = await GetHelper.DeployContractAsync(Abi,
                    Bytecode, address);
                GetHelper.GetContract(Abi, deployContract.ContractAddress);
                var resultList= await list;
                List<Task> tasks = new List<Task>();
                int count = resultList.Count;
                for (int i = 0; i < count; i++)
                {
                    tasks.Add(GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] {resultList[i] }));
                    if ((i%5==0&&i!=0)||i==count-1)
                    {
                        await Task.WhenAll(tasks);
                        tasks = new List<Task>();
                    }
                }
                Json.WriteJson(new List<ConfigStructure>()
                { new ConfigStructure() { Abi = Abi, AddressBlockChain = deployContract.ContractAddress, Bytecode = Bytecode } },
                Option.AddressConfigFileDefault);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private void OpenLoadFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel (*.xlsx)|*.xlsx";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                FileAddress = dlg.FileName;
            }

        }
       
    }
}
