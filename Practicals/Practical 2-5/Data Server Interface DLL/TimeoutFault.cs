using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Data_Server_Interface_DLL
{
    [DataContract]
    public class TimeoutFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
