using Data_Server_Interface_DLL;
using Library;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business_tier
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false,
        IncludeExceptionDetailInFaults = true)]
    internal class BusinessServer : BusinessServerInterface
    {
        private uint LogNumber = 0;
        private readonly DataServerInterface channel;
        private readonly ChannelFactory<DataServerInterface> serverInterface;

        readonly static Dictionary<string, DataStruct> cache = new Dictionary<string, DataStruct>();
        readonly static LinkedList<string> lru = new LinkedList<string>();
        int maxCacheSize = 10;

        public BusinessServer()
        {
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";

            serverInterface = new ChannelFactory<DataServerInterface>(tcp, URL);
            tcp.MaxReceivedMessageSize = 10 * 1024 * 1024;

            tcp.SendTimeout = TimeSpan.FromSeconds(7);
            tcp.ReceiveTimeout = TimeSpan.FromSeconds(7);
            tcp.OpenTimeout = TimeSpan.FromSeconds(2);
            tcp.CloseTimeout = TimeSpan.FromSeconds(2);

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
                Log("ERROR: " + ex.Detail.Message);
                var errorDataStruct = new DataStruct
                {
                    firstName = "INDEX",
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
            // Create the default DS
            DataStruct result = new DataStruct
            {
                firstName = "NOT FOUND",
                lastName = "NOT FOUND",
                pin = 0,
                acctNo = 0,
                balance = 0
            };

            try { 

                Log("Client has requested info on last name " + searchLastName);

                searchLastName = searchLastName.Trim().ToLower();

                // Check the cache
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

                    // If not in the cache, linear search.
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
                    

                    // Cache stuff
                    cache.Add(searchLastName, result);
                    lru.AddFirst(searchLastName);

                    Log($"Cache miss. Adding entry to cache. Cache entries now: {cache.Count}");

                if (cache.Count > maxCacheSize)
                    {
                        Log("Cache at maximum size. Removing least recently used entry.");
                        cache.Remove(lru.Last());
                    }
                    return result;
                } 

            catch (TimeoutException e)
            {
                Log("Timeout exception occurred: " +  e.Message);
                throw new FaultException<TimeoutFault>(new TimeoutFault { Message = "Timeout occured!" });
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logMessage)
        {
            string time = DateTime.Now.ToString();
            string logNumberString = LogNumber.ToString();
            Console.WriteLine($"LOG #{logNumberString} - {time} : {logMessage}");
            LogNumber++;
        }
       
    }
}
