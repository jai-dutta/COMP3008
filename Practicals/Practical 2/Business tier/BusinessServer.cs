using Library;
using Server;
using ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business_tier
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false,
        IncludeExceptionDetailInFaults = true)]
    internal class BusinessServer : BusinessServerInterface
    {
        private uint LogNumber = 0;
        private readonly DataServerInterface channel;
        private readonly ChannelFactory<DataServerInterface> serverInterface;

        readonly Dictionary<string, DataStruct> cache = new Dictionary<string, DataStruct>();
        readonly LinkedList<string> lru = new LinkedList<string>();
        int maxCacheSize = 10;

        public BusinessServer()
        {
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";

            serverInterface = new ChannelFactory<DataServerInterface>(tcp, URL);
            tcp.MaxReceivedMessageSize = 10 * 1024 * 1024;
            channel = serverInterface.CreateChannel();
            Log($"Created channel to Data Server on connection {URL} with TCP message size of {tcp.MaxReceivedMessageSize / 1024} KB");
        }

        public int GetNumEntries()
        {
            int entries = channel.GetNumEntries();
            Log($"Client has requested total number of entries ({entries})");
            return entries;
        }

        public DataStruct GetValuesForEntry(int index)
        {

            Log("Client has requested info on index " + index);
            try 
            {
                var data = channel.GetValuesForEntry(index);
                Console.WriteLine("Requested index found with account number: " + data.acctNo + "\n");
                return data;
            }
            catch (FaultException<IndexFault> ex)
            {
                Log("ERROR: " + ex.Message);
                var errorDataStruct = new DataStruct
                {
                    firstName = "ERROR",
                    lastName = "ERROR",
                    pin = 0,
                    acctNo = 0,
                    balance = 0
                };
                return errorDataStruct;
            }
        }

        public DataStruct SearchForLastName(string searchLastName)
        {
            Log("Client has requested info on last name " + searchLastName);

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
                Log("Cache hit.");
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
            Console.WriteLine();
            Log("Search finished.");

            Log($"Cache miss. Adding entry to cache. Cache entries now: {cache.Count}");
            cache.Add(searchLastName, result);
            lru.AddFirst(searchLastName);
            if (cache.Count > maxCacheSize)
            {
                Log("Cache at maximum size. Removing least recently used entry.");
                cache.Remove(lru.Last());
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logMessage)
        {
            string time = DateTime.Now.ToString();
            string logNumber = LogNumber.ToString();
            Console.WriteLine($"LOG #{logNumber} - {time} : {logMessage}");
        }
       
    }
}
