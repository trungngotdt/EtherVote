using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumVoting.Utilities
{
    public interface IRegisterParamaters
    {
        object GetParamater(string nameOfPara);

        void SetParamater(string nameOfPara, object value);
    }
}
