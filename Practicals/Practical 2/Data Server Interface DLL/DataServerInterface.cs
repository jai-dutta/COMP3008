using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerDLL
{

    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        int GetNumEntries();

        [OperationContract]
        [FaultContract(typeof(IndexFault))]
        void GetValuesForEntry(int index, out string firstName, out string lastName, out uint pin, out uint acctNo, out int balance);

        [OperationContract]
        List<DataStruct> GetAllValues();
    }
}
