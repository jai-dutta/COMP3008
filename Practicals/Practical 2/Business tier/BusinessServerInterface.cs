using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business_tier
{
    [ServiceContract]
    public interface BusinessServerInterface
    {
        [OperationContract]
        void GetValuesForEntry(int index, out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance);

        [OperationContract]
        DataStruct SearchForLastName(string searchLastName);

        [OperationContract]
        int GetNumEntries();

        
    }
}
