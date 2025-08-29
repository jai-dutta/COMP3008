using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Library;
using ServerDLL;

namespace Server
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    internal class DataServer : DataServerInterface
    {
        Database database;

        public DataServer()
        {
            database = new Database();
        }

        public int GetNumEntries()
        {
            return database.GetNumRecords();
        }

        public DataStruct GetValuesForEntry(int index)
        {

            if (index < 0 || index >= database.GetNumRecords())
            {
                throw new FaultException<IndexFault>(
                    new IndexFault
                    {
                        Fault = $"Index {index} out of range! Accepted indices between 0 and {database.GetNumRecords() - 1}"
                    });
            }

            var firstName = database.getFirstNameByIndex(index);
            var lastName = database.getLastNameByIndex(index);
            var pin = database.GetPINByIndex(index);
            var acctNo = database.GetAcctNoByIndex(index);
            var balance = database.GetBalanceByIndex(index);
            Console.WriteLine("Client has requested info on index " + index);

            DataStruct dataStruct = new DataStruct { 
                firstName = firstName,
                lastName = lastName, pin = pin,
                acctNo = acctNo, balance = balance

            };
            return dataStruct;
        }

        public List<DataStruct> GetAllValues()
        {
            return database.GetAllDataStructs();
        }
    }
}
