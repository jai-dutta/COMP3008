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
        DataStruct GetValuesForEntry(int index);

        [OperationContract]
        DataStruct SearchForLastName(string searchLastName);

        [OperationContract]
        int GetNumEntries();

        
    }
}
