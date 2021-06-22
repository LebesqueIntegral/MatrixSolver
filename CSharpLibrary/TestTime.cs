using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;

namespace CSharpLibrary
{
    [Serializable]
    public class TestTime
    {
        List<string> info { get; set; }
        public TestTime()
        {
            info = new List<string>();
        }

        public void Add(string s)
        {
            info.Add(s);
        }

        public override string ToString()
        {
            string s = "";
            foreach (var item in info)
                s += item;
            return s;
        }

        public static bool Save(string filename, TestTime obj)
        {
            FileStream fileStream = null;
            bool finish = false;
            try
            {
                fileStream = File.Create(filename);
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(fileStream, obj);
                finish = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение в Save: " + ex.Message);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return finish;
        }
        public static bool Load(string filename, ref TestTime obj)
        {
            FileStream fileStream = null;
            bool finish = false;
            try
            {
                fileStream = File.OpenRead(filename);
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                obj = binaryFormatter.Deserialize(fileStream) as TestTime;
                finish = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение в Load: " + ex.Message);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
            return finish;
        }
    }
}
