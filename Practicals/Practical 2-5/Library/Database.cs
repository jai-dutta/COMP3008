using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Database
    {
        List<DataStruct> dataStructs;
        DatabaseGenerator generator;
        public Database()
        {
            
            dataStructs = new List<DataStruct>();
            generator = new DatabaseGenerator();
            
            for (int i = 0; i < 100000; i++)
            {

                generator.GetNextAccount(out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance);

                DataStruct dataStruct = new DataStruct();

                dataStruct.firstName = firstName;
                dataStruct.lastName = lastName;
                dataStruct.pin = pin;
                dataStruct.acctNo = acctNo;
                dataStruct.balance = balance;

                dataStructs.Add(dataStruct);
            }
            dataStructs = dataStructs.OrderBy(ds => ds.acctNo).ToList();
        }

        public string getFirstNameByIndex(int index)
        {
            return dataStructs[index].firstName;
        }

        public string getLastNameByIndex(int index)
        {
            return dataStructs[index].lastName;
        }

        public uint GetAcctNoByIndex(int index)
        {
            return dataStructs[index].acctNo;
        }

        public uint GetPINByIndex(int index)
        {
            return dataStructs[index].pin;
        }

        public int GetBalanceByIndex(int index)
        {
            return dataStructs[index].balance;
        }
        public int GetNumRecords()
        {
            return dataStructs.Count;
        }

        public List<DataStruct> GetAllDataStructs()
        {
            return dataStructs;
        }

    }
}
