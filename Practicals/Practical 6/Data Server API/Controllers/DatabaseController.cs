using DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library;
namespace Data_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly Database _database;

        public DatabaseController(Database database)
        {
            _database = database;
        }

        // GET api/Database/
        [HttpGet]
        public IActionResult GetNumEntries()
        {
            return Ok(_database.GetNumRecords());
        }

        // GET api/Database/{index}
        [HttpGet("{index}")]
        public IActionResult GetValuesForEntry(int index)
        {
            if (index < 0 || index >= _database.GetNumRecords())
            {
                return NotFound(new { Message = $"Index {index} out of range" });
            }
            var firstName = _database.getFirstNameByIndex(index);
            var lastName = _database.getLastNameByIndex(index);
            var pin = _database.GetPINByIndex(index);
            var acctNo = _database.GetAcctNoByIndex(index);
            var balance = _database.GetBalanceByIndex(index);

            var dataStruct = new DataStruct
            {
                firstName = firstName, lastName = lastName,
                pin = pin, acctNo = acctNo, balance = balance
            };

            return Ok(DataStructDto.MapDataStructDto(dataStruct));
        }

        // GET api/Database/all
        [HttpGet("all")]
        public IActionResult GetAllValues()
        {
            var dataStructDtoList = new List<DataStructDto>();
            foreach (var ds in _database.GetAllDataStructs())
            {
                dataStructDtoList.Add(DataStructDto.MapDataStructDto(ds));
            }
            return Ok(dataStructDtoList);
        }
    }
}
