using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public interface IWorkJson
    {
        void WriteJson(List<ConfigStructure> configs, string addressFile);


        List<ConfigStructure> ReadJson(string addressFile);
    }
}
