using ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business_tier
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- BUSINESS SERVER BOOTING ---");

            ServiceHost host = new ServiceHost(typeof(BusinessServer));

            NetTcpBinding tcp = new NetTcpBinding();

            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://localhost:8101/BusinessServer");
            host.Open();

            Console.WriteLine("--- BUSINESS SERVER ONLINE ---" +
                "\n PRESS ANY BUTTON TO EXIT...");
            Console.ReadLine();
            Console.WriteLine("EXITING...");
            System.Threading.Thread.Sleep(500);
            host.Close();

        }
    }
}
