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
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
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

        public void GetValuesForEntry(int index, out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance)
        {
            if (index < 0 || index >= database.GetNumRecords())
            {
                throw new FaultException<IndexFault>(
                    new IndexFault
                    {
                        Fault = $"Index {index} out of range! Accepted indices between 0 and {database.GetNumRecords() - 1}"
                    });
            }


            firstName = database.getFirstNameByIndex(index);
            lastName = database.getLastNameByIndex(index);
            pin = database.GetPINByIndex(index);
            acctNo = database.GetAcctNoByIndex(index);
            balance = database.GetBalanceByIndex(index);
            Console.WriteLine("Client has requested info on index " + index);

            DataStruct dataStruct = new DataStruct { 
                firstName = firstName,
                lastName = lastName, pin = pin,
                acctNo = acctNo, balance = balance

            };
        }

        public List<DataStruct> GetAllValues()
        {
            return database.GetAllDataStructs();
        }
    }
}
