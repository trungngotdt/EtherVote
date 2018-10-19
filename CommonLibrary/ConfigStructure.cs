using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class ConfigStructure
    {
        private string abi;
        private string bytecode;
        private string addressBlockChain;

        public string Abi { get => abi; set => abi = value; }
        public string Bytecode { get => bytecode; set => bytecode = value; }
        public string AddressBlockChain { get => addressBlockChain; set => addressBlockChain = value; }
    }
}
