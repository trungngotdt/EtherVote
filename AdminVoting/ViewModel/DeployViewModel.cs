using AdminVoting.View;
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
        private bool isOpenDialog;
        private object contentDialog;
        private bool isOpenSbNotify;
        private string messageSbNotify;

        private PropertiesOption option;

        private List<string> candidates;

        private IHelper helper;

        public ICommand CommandBtnDeployClick { get; set; }
        public ICommand CommandBtnOpenFile { get; set; }


        public IHelper GetHelper { get => helper; }
        public IHelperMongo HelperMongo { get ; set ; }
        public IRegisterParamaters RegisterParamaters { get ; set ; }
        public string Abi { get => abi; set => abi = value; }
        public string Bytecode { get => bytecode; set => bytecode = value; }



        public List<string> Candidates { get => candidates; set => candidates = value; }
        public string FileAddress { get => fileAddress; set { fileAddress = value; RaisePropertyChanged("FileAddress"); } }
        public IWorkExcel Excel { get ; set ; }
        public IWorkJson Json { get ; set ; }


        public bool IsOpenDialog { get => isOpenDialog; set { isOpenDialog = value; RaisePropertyChanged("IsOpenDialog"); } }
        public object ContentDialog { get => contentDialog; set { contentDialog = value; RaisePropertyChanged("ContentDialog"); } }
        public bool IsOpenSbNotify { get => isOpenSbNotify; set { isOpenSbNotify = value; RaisePropertyChanged("IsOpenSbNotify"); } }
        public string MessageSbNotify { get => messageSbNotify; set { messageSbNotify = value; RaisePropertyChanged("MessageSbNotify"); } }

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
            this.RegisterParamaters = _registerParamaters;
            this.helper = _helper;
            this.HelperMongo = _helperMongo;
            this.Excel = workExcel;
            this.Json = workJson;
            Init();
        }

        

        private void Init()
        {
            OpenDialog(true);
            address = RegisterParamaters.GetParamater("address").ToString();
            InitCommand();
            OpenDialog(false);
        }

        private void InitCommand()
        {
            CommandBtnDeployClick = new RelayCommand(async () => { await InitContractVotingAsync(); });
            CommandBtnOpenFile = new RelayCommand(() => { OpenLoadFile(); });
        }


        private async Task InitContractVotingAsync()
        {
            try
            {
                OpenDialog(true);
                var validateInfo =String.IsNullOrEmpty( FileAddress) || String.IsNullOrEmpty(Abi) || String.IsNullOrEmpty(Bytecode);
                if (validateInfo)
                {
                    OpenSnackBarNotify(true, "Empty value");
                    OpenDialog(false);
                    return;
                }
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
                {
                    new ConfigStructure() { Abi = Abi, AddressBlockChain = deployContract.ContractAddress, Bytecode = Bytecode }
                },Option.AddressConfigFileDefault);
                OpenSnackBarNotify(false,"");
                OpenDialog(false);
            }
            catch (System.Exception ex)
            {
                OpenDialog(false);
                OpenSnackBarNotify(true, ex.Message);
                throw ex;
            }
            
        }

        private void OpenLoadFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel (*.xlsx)|*.xlsx"; 
            Nullable<bool> result = dlg.ShowDialog(); 
            if (result == true)
            {
                FileAddress = dlg.FileName;
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
    }
}
