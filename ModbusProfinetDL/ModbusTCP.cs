using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NModbus;

namespace BetonarkaDL
{
    public static class ModbusTCP
    {
        //public static string ipAddr = "213.215.84.85";
        //public static string ipAddr = "192.168.1.201"; // Knezpole
        //public static int ipPort = 8881;
        public static string ipAddrMiesacka1 = ConfigurationManager.AppSettings["ipAdresaBetonarka1"];
        public static string ipAddrMiesacka2 = ConfigurationManager.AppSettings["ipAdresaBetonarka2"];
        public static int portBetonarka1 = int.Parse(ConfigurationManager.AppSettings["portBetonarka1"]);
        public static int portBetonarka2 = int.Parse(ConfigurationManager.AppSettings["portBetonarka2"]);
        public static byte slaveId = 0;

        public static List<double> MasterReadDoubleWords(ushort startAddress, ushort numDoubleWords, int miesacka)
        {
            try
            {
                string ipAddr;
                int port;
                if (miesacka == 1)
				{
                    ipAddr = ipAddrMiesacka1;
                    port = portBetonarka1;
                }
                else if (miesacka == 2)
				{
                    ipAddr = ipAddrMiesacka2;
                    port = portBetonarka2;
				}
                else
				{
                    return null;
				}
                using (TcpClient client = new TcpClient(ipAddr, port))
                {
                    var factory = new ModbusFactory();
                    IModbusMaster master = factory.CreateMaster(client);

                    ushort numRegisters = (ushort)(numDoubleWords * 2); // double word is across 2 registers
                    ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                    List<double> recvData = new List<double>();

                    for (int i = 0; i < numRegisters; i += 2)
                    {
                        double recvValue = registers[i] | (registers[i + 1] << 16);
                        recvData.Add(recvValue / 10); // 1 decimal value
                    }

                    return recvData;
                }
            }
            catch (Exception ex)
            {
                Library.WriteLog("Spojenie Modbus neuspesne:");
                Library.WriteLog(ex);
                return null;
            }
        }
    }
}
