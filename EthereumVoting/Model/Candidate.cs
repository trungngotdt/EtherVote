using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace EthereumVoting.Model
{
    [FunctionOutput]
    public class Candidate
    {
        private string name;
        private int numVote;
        private bool isExist;

        [Parameter("bytes32", "name", 1)]
        public string Name { get => name; set => name = value; }

        [Parameter("uint8", "numVote", 2)]
        public int NumVote { get => numVote; set => numVote = value; }

        [Parameter("bool", "isExist", 3)]
        public bool IsExist { get => isExist; set => isExist = value; }
        
        public Candidate()
        {

        }
    }
}
