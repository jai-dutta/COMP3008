using ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- DATABASE SERVER BOOTING ---");

            ServiceHost host = new ServiceHost(typeof(DataServer));

            NetTcpBinding tcp = new NetTcpBinding();
            tcp.MaxReceivedMessageSize = 10 * 1024 * 1024; 

            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://localhost:8100/DataService");
            host.Open();

            Console.WriteLine("--- DATABASE SERVER ONLINE ---" +
                "\n PRESS ANY BUTTON TO EXIT...");
            Console.ReadLine();
            Console.WriteLine("EXITING...");
            System.Threading.Thread.Sleep(500);
            host.Close();


        }
    }
}
