using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class WorkExcel:IWorkExcel
    {
        public List<object> GetData(string Address)
        {
            var fs = File.Open(Address, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(fs);
            List<object> list = new List<object>();
            object value;
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    value = reader.GetValue(i);
                    list.Add(value);
                }
            }
            return list;
        }
    }
}
