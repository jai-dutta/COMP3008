using Library;
using Server;
using ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business_tier
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    internal class BusinessServer : BusinessServerInterface
    {
        private readonly DataServerInterface channel;
        private readonly ChannelFactory<DataServerInterface> serverInterface;

        readonly Dictionary<string, DataStruct> cache = new Dictionary<string, DataStruct>();
        readonly LinkedList<string> lru = new LinkedList<string>();
        int maxCacheSize = 500;

        public BusinessServer()
        {
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";

            serverInterface = new ChannelFactory<DataServerInterface>(tcp, URL);
            tcp.MaxReceivedMessageSize = 10 * 1024 * 1024;
            channel = serverInterface.CreateChannel();
        }

        public int GetNumEntries()
        {
            return channel.GetNumEntries();
        }

        public void GetValuesForEntry(int index, out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance)
        {

            Console.WriteLine("Client has requested info on index " + index);
            try 
            { 
                channel.GetValuesForEntry(index, out firstName, out lastName, out pin, out acctNo, out balance);
            }
            catch (FaultException<IndexFault> ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                firstName = lastName = "ERROR";
                pin = acctNo = 0;
                balance = 0;
            }
        }

        public DataStruct SearchForLastName(string searchLastName)
        {
            Console.WriteLine("Client has requested info on last name " + searchLastName);
            searchLastName = searchLastName.Trim().ToLower();

            DataStruct result = new DataStruct {
                firstName= "NOT FOUND",
                lastName = "NOT FOUND",
                pin = 0,
                acctNo = 0,
                balance = 0
            };

            if (cache.ContainsKey(searchLastName))
            {
                Console.WriteLine("Cache hit.");
                result.firstName = cache[searchLastName].firstName;
                result.lastName = cache[searchLastName].lastName;
                result.pin = cache[searchLastName].pin;
                result.acctNo = cache[searchLastName].acctNo;
                result.balance = cache[searchLastName].balance;
                return result;

            }

            int index = 0;
            List<DataStruct> dsValues = channel.GetAllValues();
            foreach (DataStruct ds in dsValues)
            {
                Console.Write($"\rCurrent index: {index}");

                if (ds.lastName.Equals(searchLastName, StringComparison.OrdinalIgnoreCase))
                {
                    result = ds;
                    break;
                }
                index++;
            }
            Console.WriteLine("\nSearch finished.");

            Console.WriteLine("Cache miss. Adding entry to cache.");
            cache.Add(searchLastName, result);
            lru.AddFirst(searchLastName);
            if (cache.Count > maxCacheSize)
            {
                cache.Remove(lru.Last());
            }

            return result;


        }

    }
}
