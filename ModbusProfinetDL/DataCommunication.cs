using BetonarkaDL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonarkaDL
{
    public static class DataCommunication
    {
        static List<double> dataModbus = new List<double>(), dataModbusLast;
        static List<int> dataProfinet;
        static readonly bool vycitajBetonarka1 = int.Parse(ConfigurationManager.AppSettings["vycitajBetonarka1"]) != 0;
        static readonly bool vycitajBetonarka2 = int.Parse(ConfigurationManager.AppSettings["vycitajBetonarka2"]) != 0;
        static readonly bool vycitajPalety = int.Parse(ConfigurationManager.AppSettings["vycitajPalety"]) != 0;
        static readonly string casUlozBetonarka = ConfigurationManager.AppSettings["casUlozBetonarka"];
        static bool areDataSaved = false;

        // miesacky
        public static void ModbusTask()
        {
            while (vycitajBetonarka1 || vycitajBetonarka2)
            {
                try
				{
                    List<double> readData = new List<double>();
                    bool successMiesacka1 = false, successMiesacka2 = false;

                    // save last value
                    if (dataModbus.Count > 0)
                    {
                        dataModbusLast = new List<double>(dataModbus);
                    }

                    dataModbus = new List<double>();

                    // betonarka 1
                    if (vycitajBetonarka1)
                    {
                        // velka miesacka 1
                        readData = ModbusTCP.MasterReadDoubleWords(620, 2, 1);
                        if (readData != null)
                        {
                            dataModbus.AddRange(readData);
                            Library.WriteLastTimeModbus1();
                            successMiesacka1 = true;
                            // mala miesacka 1
                            readData = ModbusTCP.MasterReadDoubleWords(718, 1, 1);
                            if (readData != null)
                            {
                                dataModbus.AddRange(readData);
                            }
                        }
                    }

                    if (vycitajBetonarka2)
                    {
                        // velka miesacka 2
                        readData = ModbusTCP.MasterReadDoubleWords(1524, 3, 2);
                        if (readData != null)
                        {
                            // mala miesacka 2
                            dataModbus.AddRange(readData);
                            Library.WriteLastTimeModbus2();
                            successMiesacka2 = true;
                            readData = ModbusTCP.MasterReadDoubleWords(1632, 2, 2);
                            if (readData != null)
                            {
                                dataModbus.AddRange(readData);
                            }
                        }
                    }

					// save log
					CsvLayer.SaveMiesacky(dataModbus, successMiesacka1, successMiesacka2, true);

					// check and save data
					bool willSaveDataModbus = false;
                    if (DateTime.Now.ToString("HH:mm") == casUlozBetonarka)
					{
                        if (!areDataSaved)
						{
                            willSaveDataModbus = true;
						}
                    }
                    else
					{
                        areDataSaved = false;
					}
                    if (willSaveDataModbus)
                    {
                        CsvLayer.SaveMiesacky(dataModbus, successMiesacka1, successMiesacka2);
                        areDataSaved = true;
                    }
                }
                catch (Exception ex)
				{
                    Library.WriteLog(ex);
                }

                Task.Delay(5000).Wait();
            }
        }

        public static void ProfinetTask()
        {
            while (vycitajPalety)
            {
                try
				{
                    dataProfinet = ProfinetS7.ReadData();
                    CsvLayer.SavePalety(dataProfinet, true);
                    if (dataProfinet != null)
                    {
                        Library.WriteLastTimeProfinet();
                        // save log
                        CsvLayer.SavePalety(dataProfinet, true);
                        if (ProfinetS7.readyToSave == true)
                        {
                            ProfinetS7.readyToSave = false;
                            CsvLayer.SavePalety(dataProfinet);
                        }
                    }
                }
                catch (Exception ex)
				{
                    Library.WriteLog(ex);
                }

                Task.Delay(5000).Wait();
            }
        }
    }
}
