using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace IRS
{
    public static class SerializerHelper
    {
        public static void Serialize(object obj, Type type, string path, int serializationType)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            string serObject = "";
            if (serializationType == 1)
            {
                serObject = JsonSerializer.SerializeToString(obj, type);

            }
            else {
                serObject = CsvSerializer.SerializeToString(obj);
            }


            file.WriteLine(serObject);
            file.Close();
        }
    }
}
