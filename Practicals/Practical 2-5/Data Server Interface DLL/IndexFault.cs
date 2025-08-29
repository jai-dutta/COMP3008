using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerDLL
{
    [DataContract]
    public class IndexFault
    {
        [DataMember]
        public string Fault { get; set; }
    }
}