using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;
using BetonarkaDL.Models;
using CsvHelper.Configuration;
using System.Configuration;

namespace BetonarkaDL
{
    public static class CsvLayer
    {
        public const string dateFormat = "dd-MM-yyyy";
        public const string dateTimeFormat = "HH:mm:ss dd-MM-yyyy";
        public const string dateTimeFormatCsv = "HH:mm:ss_dd-MM-yyyy";
        public static readonly string dataPath = ConfigurationManager.AppSettings["cestaData"].Length > 0 
                                                ? ConfigurationManager.AppSettings["cestaData"]
                                                : AppDomain.CurrentDomain.BaseDirectory + "\\data";
        public static string lastTimeMiesacky;
        public static string lastTimePalety;

        private static string GetSavePath(DateTime now)
        {
            var ci = new CultureInfo("en-US");
            var currYear = now.ToString("yyyy");
            var currMonth = now.ToString("MMMM", ci);
            var currDate = now.ToString(dateFormat);
            var savePath = $"{dataPath}\\{currYear}\\{currMonth}\\{currDate}";
            return savePath;
        }

        public static void SaveData<T>(List<T> data, string filePath, bool writeHeader = false)
        {
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = !File.Exists(filePath) || writeHeader,
            };

            try
            {
                using (var stream = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(data);
                }
            }
            catch (Exception ex) {
                Library.WriteLog("Zapis do CSV suboru zlyhal:");
                Library.WriteLog(ex);
            }
        }

        public static void SaveMiesacky(List<double> data, bool successMiesacka1, bool successMiesacka2, bool saveLog = false)
        {
            if (!successMiesacka1 && !successMiesacka2)
			{
                return;
			}

            var currDateTime = DateTime.Now;
            lastTimeMiesacky = currDateTime.ToString(dateTimeFormat);
            var savePath = GetSavePath(currDateTime);
            if (!saveLog)
			{
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
            }

            int baseIdx = 0;

            if (successMiesacka1)
			{
                var records = new List<MiesackyModel1>
                {
                    new MiesackyModel1
                    {
                        Cas = currDateTime.ToString(dateTimeFormatCsv),
                        Miesacka1VelkaCement = data[0],
                        Miesacka1VelkaPopol = data[1],
                    }
                };
                baseIdx += 2;

                if (data.Count > 2)
                {
                    records[0].Miesacka1MalaCement = data[2];
                    baseIdx++;
                }

                var filePath = !saveLog ? $"{savePath}\\dataBetonarka1.csv" : (dataPath + "\\betonarka1.txt");
                SaveData(records, filePath, saveLog);
            }

            if (successMiesacka2)
			{
                var records = new List<MiesackyModel2>
                {
                    new MiesackyModel2
                    {
                        Cas = currDateTime.ToString(dateTimeFormatCsv),
                        Miesacka2VelkaCement = data[0 + baseIdx],
                        Miesacka2VelkaPopol = data[1 + baseIdx],
                        Miesacka2VelkaCement2 = data[2 + baseIdx],
                    }
                };

                if (data.Count > (3 + baseIdx))
                {
                    records[0].Miesacka2MalaCement = data[3 + baseIdx];
                    records[0].Miesacka2MalaBielyCement = data[4 + baseIdx];

                }

                var filePath = !saveLog ? $"{savePath}\\dataBetonarka2.csv" : (dataPath + "\\betonarka2.txt");
                SaveData(records, filePath, saveLog);
            }
        }

        public static void SavePalety(List<int> data, bool saveLog = false)
        {
            var currDateTime = DateTime.Now;
            lastTimePalety = currDateTime.ToString(dateTimeFormat);
            var savePath = GetSavePath(currDateTime);
            if (!saveLog)
            {
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
            }
            var records = new List<PaletyModel>
            {
                new PaletyModel
                {
                    Cas = currDateTime.ToString(dateTimeFormatCsv),
                    ProgramTERAMEX = data[0],
                    VrstevPaleta = data[1],
                    ProgramHESS = data[2],
                    DenniPocitadlo = data[3],
                    KontrolniCiselnik = data[4],
                    Farba = data[5],
                }
            };

            var filePath = !saveLog ? $"{savePath}\\dataPalety.csv" : (dataPath + "\\palety.txt");
            SaveData(records, filePath, saveLog);
        }
    }
}
