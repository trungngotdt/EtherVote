using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public interface IWorkExcel
    {
        List<object> GetData(string Address);
    }
}
