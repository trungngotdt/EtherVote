using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumVoting.Utilities
{
    public class RegisterParamaters:IRegisterParamaters
    {
        private Dictionary<string, object> paramater;

        public object GetParamater(string nameOfPara)
        {            
            return paramater[nameOfPara];
        }

        public void SetParamater(string nameOfPara,object value)
        {
            if(paramater.ContainsKey(nameOfPara))
            {
                paramater[nameOfPara] = value;
                return;
            }
            paramater.Add(nameOfPara, value);
        }
    }
}
