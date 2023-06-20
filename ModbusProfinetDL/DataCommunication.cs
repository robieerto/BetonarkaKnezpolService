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

                    // check and save data
                    bool willSaveDataModbus = false;
                    if (dataModbus != null && dataModbusLast != null)
                    {
                        for (int i = 0; i < dataModbus.Count; i++)
                        {
                            if (dataModbus[i] < dataModbusLast[i])
                            {
                                willSaveDataModbus = true;
                                break;
                            }
                        }
                    }
                    if (willSaveDataModbus)
                    {
                        CsvLayer.SaveMiesacky(dataModbus, successMiesacka1, successMiesacka2);
                    }
                }
                catch (Exception ex)
				{
                    Library.WriteLog(ex);
                }

                Task.Delay(2000);
            }
        }

        public static void ProfinetTask()
        {
            while (vycitajPalety)
            {
                try
				{
                    dataProfinet = ProfinetS7.ReadData();
                    if (dataProfinet != null)
                    {
                        Library.WriteLastTimeProfinet();
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

                Task.Delay(2000);
            }
        }
    }
}
