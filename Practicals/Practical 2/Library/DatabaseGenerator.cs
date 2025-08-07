using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    internal class DatabaseGenerator
    {
        readonly Random rand = new Random();

        List<string> firstNames = new List<string> 
        {
            "John", "Steve", "Matthew", "Adam", "Bob", "Bartholomew", "Jack"
        };

        List<string> lastNames = new List<string>
        {
            "Smith", "Green", "Woods", "Black", "Harding", "Morrison"
        };

        private string GetFirstname()
        {
            int indice = rand.Next(firstNames.Count);

            return firstNames[indice];
        }

        private string GetLastname()
        {
            int indice = rand.Next(lastNames.Count);

            return lastNames[indice];
        }

        private uint GetPIN()
        {
            return (uint)rand.Next(0, 10000);
        }

        private uint GetAcctNo()
        {
            return (uint)(rand.Next(0, 10000000));
        }

        private int GetBalance()
        {
            return (rand.Next(-1000, 5000));
        }

        public void GetNextAccount(out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance)
        {
            firstName = GetFirstname();
            lastName = GetLastname();
            pin = GetPIN();
            acctNo = GetAcctNo();
            balance = GetBalance();
        }
    }
}
