using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [DataContract]
    public class DataStruct
    {
        [DataMember]
        public string firstName = "";
        [DataMember]
        public string lastName = "";
        [DataMember]
        public uint pin = 0;
        [DataMember]
        public uint acctNo = 0;
        [DataMember]
        public int balance = 0;

    }
}
