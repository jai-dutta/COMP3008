using System.Runtime.Serialization;

namespace Data_Server_Interface_DLL
{
    [DataContract]
    public class IndexFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}