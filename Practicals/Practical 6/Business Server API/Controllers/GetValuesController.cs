using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using DTOS;  // DataIntermed or DataStructDto

namespace Business_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetValuesController : ControllerBase
    {
        private readonly RestClient _client;

        public GetValuesController()
        {
            _client = new RestClient("http://localhost:5050/api/Database/");
        }

        // GET api/getvalues/{index}
        [HttpGet("{index}")]
        public IActionResult GetByIndex(int index)
        {
            var response = _client.Execute(new RestRequest($"{index}", Method.Get));
            if (!response.IsSuccessful)
                return StatusCode((int)response.StatusCode, response.Content);

            var data = JsonConvert.DeserializeObject<DataStructDto>(response.Content!);
            return Ok(data);
        }
    }
}