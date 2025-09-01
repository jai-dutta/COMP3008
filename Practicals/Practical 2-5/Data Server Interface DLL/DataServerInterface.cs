using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Data_Server_Interface_DLL
{

    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        int GetNumEntries();

        [OperationContract]
        [FaultContract(typeof(IndexFault))]
        DataStruct GetValuesForEntry(int index);

        [OperationContract]
        List<DataStruct> GetAllValues();
    }
}
