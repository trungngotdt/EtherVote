using CommonLibrary;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EthereumVoting.ViewModel
{
    public class ConfigViewModel:ViewModelBase
    {
        private string abi;
        private string bytecode;
        private string addressFile;
        private string addressContract;
        private bool isOpenSbNotify;
        private string messageSbNotify;

        private IWorkJson workjson;

        private PropertiesOption option;

        private ICommand commandBtnImportConfig;

        public string Abi { get => abi; set  {abi = value;Option.Abi = value; RaisePropertyChanged("Abi"); } }
        

        public string ByteCode { get => bytecode; set  {bytecode = value;Option.ByteCode = value; RaisePropertyChanged("ByteCode");} }
        public string AddressFile { get => addressFile; set {addressFile = value;Option.AddressConfigFileDefault = value; RaisePropertyChanged("AddressFile");} }
        public string AddressContract { get => addressContract; set { addressContract = value;Option.AddressContract = value; RaisePropertyChanged("AddressContract");} }
        
        public ICommand CommandBtnImportConfig => commandBtnImportConfig = new RelayCommand(() => { ImportFile(); });

        public IWorkJson Workjson { get => workjson; set => workjson = value; }

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

        public bool IsOpenSbNotify { get => isOpenSbNotify; set => isOpenSbNotify = value; }
        public string MessageSbNotify { get => messageSbNotify; set => messageSbNotify = value; }

        public ConfigViewModel(IWorkJson json)
        {
            this.workjson = json;
            Init();
        }

        private void Init()
        {
            if (File.Exists(Option.AddressConfigFileDefault))
            {
                Abi = Option.Abi;
                ByteCode = Option.ByteCode;
                AddressContract = Option.AddressContract;
                AddressFile = Option.AddressConfigFileDefault;
            }            
        }

        private void ImportFile()
        {
            try
            {               
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".json";
                dlg.Filter = "Json (*.json)|*.json";
                Nullable<bool> result = dlg.ShowDialog();
                string fileAddress = String.Empty;
                if (result==false)
                {
                    return;
                }
                fileAddress = dlg.FileName;
                var resultRead = Workjson.ReadJson(fileAddress);
                Task taskFile = Task.Factory.StartNew(() =>
                {
                    if (File.Exists(Option.AddressConfigFileDefault))
                    {
                        File.Delete(Option.AddressConfigFileDefault);
                    }
                    AddressFile = Option.AddressConfigFileDefault;
                    Workjson.WriteJson(resultRead, Option.AddressConfigFileDefault);
                });
                Abi = resultRead[0].Abi;
                ByteCode = resultRead[0].Bytecode;
                AddressContract = resultRead[0].AddressBlockChain;
            }
            catch (Exception ex)
            {
                OpenSnackBarNotify(true, ex.Message);
                throw ex;
            }
           
        }

        private void OpenSnackBarNotify(bool isOpen, string message)
        {
            IsOpenSbNotify = isOpen;
            MessageSbNotify = message;
        }

    }
}
