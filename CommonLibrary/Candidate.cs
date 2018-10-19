using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System.ComponentModel;

namespace CommonLibrary
{
    [FunctionOutput]
    public class Candidate:INotifyPropertyChanged
    {
        private string name;
        private int numVote;
        private bool isExist;
        private bool isCheck;
        private bool isEnable;

        public event PropertyChangedEventHandler PropertyChanged;

        [Parameter("bytes32", "name", 1)]
        public string Name { get => name; set => name = value; }

        [Parameter("uint8", "numVote", 2)]
        public int NumVote { get => numVote; set { numVote = value; OnPropertyChanged("NumVote"); } }

        [Parameter("bool", "isExist", 3)]
        public bool IsExist { get => isExist; set => isExist = value; }
        public bool IsCheck { get => isCheck; set => isCheck = value; }
        public bool IsEnable { get => isEnable; set { isEnable = value;OnPropertyChanged("IsEnable");  } }
        
        public Candidate()
        {

        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
