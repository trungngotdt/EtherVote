using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class PropertiesOption
    {
        private static volatile PropertiesOption instance;
        private static object syncRoot = new object();



        private string abi;
        private string byteCode;
        private string addressContract;
        private string addressRootFolder;
        private string addressConfigFileDefault;
        private string ipAddressMongoDefault;
        private string nameUserMongoDefault;
        private string passUserMongoDefault;
        private int portMongoDefault;
        private string nameOfDBMongoDefault;


        public string Abi { get => abi; set => abi = value; }
        public string ByteCode { get => byteCode; set => byteCode = value; }
        public string AddressContract { get => addressContract; set => addressContract = value; }
        public string AddressConfigFileDefault { get => addressConfigFileDefault; set => addressConfigFileDefault = value; }


        private PropertiesOption()
        {
            ipAddressMongoDefault = "127.0.0.1";
            nameUserMongoDefault = "user1";
            passUserMongoDefault = "pass1";
            PortMongoDefault = 27017;
            AddressRootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+ @"\Config";
            AddressConfigFileDefault = AddressRootFolder + @"\config.json";
            nameOfDBMongoDefault = "data1";
        }

        public static PropertiesOption Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PropertiesOption();
                    }
                }

                return instance;
            }
        }

        public string IpAddressMongoDefault { get => ipAddressMongoDefault; set => ipAddressMongoDefault = value; }
        public string NameUserMongoDefault { get => nameUserMongoDefault; set => nameUserMongoDefault = value; }
        public string PassUserMongoDefault { get => passUserMongoDefault; set => passUserMongoDefault = value; }
        public int PortMongoDefault { get => portMongoDefault; set => portMongoDefault = value; }
        public string NameOfDBMongoDefault { get => nameOfDBMongoDefault; set => nameOfDBMongoDefault = value; }
        public string AddressRootFolder { get => addressRootFolder; set => addressRootFolder = value; }
    }
}
