using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetonarkaDL
{
    public static class DataCommunication
    {
        static List<double> dataModbus, dataModbusLast;
        static List<int> dataProfinet;

        // miesacky
        public static void ModbusTask()
        {
            while (true)
            {
                // save last value
                if (dataModbus != null)
                {
                    dataModbusLast = new List<double>(dataModbus);
                }
                // velka miesacka 1
                dataModbus = ModbusTCP.MasterReadDoubleWords(620, 2, 1);
                if (dataModbus != null)
                {
                    Library.WriteLastTimeModbus();
                    // mala miesacka 1
                    var nextData = ModbusTCP.MasterReadDoubleWords(718, 1, 1);
                    if (nextData != null)
                    {
                        // velka miesacka 2
                        dataModbus.AddRange(nextData);
                        var nextData2 = ModbusTCP.MasterReadDoubleWords(1524, 3, 2);
                        if (nextData2 != null)
						{
                            // mala miesacka 2
                            dataModbus.AddRange(nextData2);
                            var nextData3 = ModbusTCP.MasterReadDoubleWords(1632, 2, 2);
                            if (nextData3 != null)
							{
                                dataModbus.AddRange(nextData3);
                            }
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
                    CsvLayer.SaveMiesacky(dataModbus);
                }

                Task.Delay(2000);
            }
        }

        public static void ProfinetTask()
        {
            while (true)
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

                Task.Delay(2000);
            }
        }
    }
}
