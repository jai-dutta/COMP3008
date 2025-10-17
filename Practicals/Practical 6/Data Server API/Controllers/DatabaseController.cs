using DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library;
namespace Data_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController(Database database) : ControllerBase
    {


        // GET api/Database/
        [HttpGet]
        public IActionResult GetNumEntries()
        {
            return Ok(database.GetNumRecords());
        }

        // GET api/Database/{index}
        [HttpGet("{index}")]
        public IActionResult GetValuesForEntry(int index)
        {
            if (index < 0 || index >= database.GetNumRecords())
            {
                return NotFound(new { Message = $"Index {index} out of range" });
            }
            var firstName = database.getFirstNameByIndex(index);
            var lastName = database.getLastNameByIndex(index);
            var pin = database.GetPINByIndex(index);
            var acctNo = database.GetAcctNoByIndex(index);
            var balance = database.GetBalanceByIndex(index);
            var profilePicture = database.GetProfilePictureByIndex(index);

            var dataStruct = new DataStruct
            {
                firstName = firstName, lastName = lastName,
                pin = pin, acctNo = acctNo, balance = balance, profilePicture = profilePicture
            };

            return Ok(DataStructDto.MapDataStructDto(dataStruct));
        }

        // GET api/Database/all
        [HttpGet("all")]
        public IActionResult GetAllValues()
        {
            var dataStructDtoList = new List<DataStructDto>();
            foreach (var ds in database.GetAllDataStructs())
            {
                dataStructDtoList.Add(DataStructDto.MapDataStructDto(ds));
            }
            return Ok(dataStructDtoList);
        }
    }
}
