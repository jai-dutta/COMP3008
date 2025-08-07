using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    [ServiceContract]
    public interface ServerInterface
    {
        [OperationContract]
        int GetNumEntries();

        [OperationContract]
        void GetValuesForEntry(int index, out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance);
    }
}
