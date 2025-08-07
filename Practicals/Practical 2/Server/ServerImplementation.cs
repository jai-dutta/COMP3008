using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class ServerImplementation : ServerInterface
    {
        Database database;

        public ServerImplementation()
        {
            database = new Database();
        }

        public int GetNumEntries()
        {
            return database.GetNumRecords();
        }

        public void GetValuesForEntry(int index, out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance)
        {
            firstName = database.getFirstNameByIndex(index);
            lastName = database.getLastNameByIndex(index);
            pin = database.GetPINByIndex(index);
            acctNo = database.GetAcctNoByIndex(index);
            balance = database.GetBalanceByIndex(index);
            Console.WriteLine("Client has requested info on index " + index);
        }
    }
}
