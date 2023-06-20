using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BetonarkaDL
{
    public static class Library
    {
        public static void WriteLog(Exception ex)
        {
            try
            {
                using (var sw = new StreamWriter(CsvLayer.dataPath + "\\log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                }

            }
            catch
            {
            }
        }

        public static void WriteLog(string Message)
        {
            try
            {
                using (var sw = new StreamWriter(CsvLayer.dataPath + "\\log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + Message);

                }
            }
            catch
            {
            }
        }

        public static void WriteLastTimeModbus1()
        {
            try
            {
                using (var sw = new StreamWriter(CsvLayer.dataPath + "\\betonarka1.txt", false))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": Posledne vycitane data");

                }
            }
            catch
            {
            }
        }

        public static void WriteLastTimeModbus2()
        {
            try
            {
                using (var sw = new StreamWriter(CsvLayer.dataPath + "\\betonarka2.txt", false))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": Posledne vycitane data");

                }
            }
            catch
            {
            }
        }

        public static void WriteLastTimeProfinet()
        {
            try
            {
                using (var sw = new StreamWriter(CsvLayer.dataPath + "\\palety.txt", false))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": Posledne vycitane data");

                }
            }
            catch
            {
            }
        }
    }
}
