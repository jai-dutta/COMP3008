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
            Console.WriteLine("--- SERVER BOOTING ---");

            ServiceHost host = new ServiceHost(typeof(ServerImplementation));

            NetTcpBinding tcp = new NetTcpBinding();

            host.AddServiceEndpoint(typeof(ServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();

            Console.WriteLine("--- SERVER ONLINE ---");
            Console.ReadLine();
            host.Close();


        }
    }
}
