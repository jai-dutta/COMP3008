using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class DataStruct
    {
        public string firstName { get; set; } = String.Empty;
        public string lastName { get; set; } = String.Empty;
        public uint pin { get; set; } = 0;
        public uint acctNo { get; set; } = 0;
        public int balance { get; set; } = 0;

    }
}
