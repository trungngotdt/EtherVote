using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class WorkJson:IWorkJson
    {
        public void WriteJson(List<ConfigStructure> configs,string addressFile)
        {
            string json = JsonConvert.SerializeObject(configs);

            //write string to file
            System.IO.File.WriteAllText(addressFile, json);
        }

        public List< ConfigStructure> ReadJson(string addressFile)
        {
            List<ConfigStructure> items;
            using (StreamReader r = new StreamReader(addressFile))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<ConfigStructure>>(json);
            }
            return items;
        }
        
    }
}
