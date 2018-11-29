using GalaSoft.MvvmLight;
using CommonLibrary;
using System.Collections.Generic;
using CommonLibrary.HelperMongo;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using CommonServiceLocator;
using System.Threading.Tasks;
using System;

namespace FastConfig.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
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


        public ICommand CommandBtnDeployClick => commandBtnDeployClick = new RelayCommand(async () => { await InitContractVotingAsync(); });

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


        public MainViewModel(IHelper _helper, IHelperMongo _helperMongo, IRegisterParamaters _registerParamaters, IWorkExcel workExcel, IWorkJson workJson)
        {
            this.registerParamaters = _registerParamaters;
            this.helper = _helper;
            this.helperMongo = _helperMongo;
            this.excel = workExcel;
            this.json = workJson;
            Option.AddressConfigFileDefault = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+ @"\Example";
            Option.AddressConfigFileDefault = Option.AddressRootFolder + @"\config.json";
            Init();
        }

        private void Init()
        {
            address = "0x12890d2cce102216644c59daE5baed380d84830c";//registerParamaters.GetParamater("address").ToString();
            GetHelper.GetWeb3(ServiceLocator.Current.GetInstance<string>("link"));
        }

        private async Task InitContractVotingAsync()
        {
            try
            {
                Task<List<object>> list = Task.Factory.StartNew<List<object>>(() => Excel.GetData(FileAddress));
                var deployContract = await GetHelper.DeployContractAsync(Abi,
                    Bytecode, address);
                GetHelper.GetContract(Abi, deployContract.ContractAddress);
                var resultList = await list;
                List<Task> tasks = new List<Task>();
                int count = resultList.Count;
                for (int i = 0; i < count; i++)
                {
                    tasks.Add(GetHelper.SendTransactionFunctionAsync(address, "SetCandidate", new object[] { resultList[i] }));
                    if ((i % 5 == 0 && i != 0) || i == count - 1)
                    {
                        await Task.WhenAll(tasks);
                        tasks = new List<Task>();
                    }
                }
                Json.WriteJson(new List<ConfigStructure>()
                { new ConfigStructure() { Abi = Abi, AddressBlockChain = deployContract.ContractAddress, Bytecode = Bytecode } },
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Example"+ @"\config.json");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private void OpenLoadFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xlsx",
                Filter = "Excel (*.xlsx)|*.xlsx"
            };

            Nullable<bool> result = dlg.ShowDialog(); 
            if (result == true)
            {
                // Open document 
                FileAddress = dlg.FileName;
            }

        }

    }
}