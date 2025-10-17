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

        [HttpGet("{index}")]
        public IActionResult GetByIndex(int index)
        {
            var response = _client.Execute(new RestRequest($"{index}", Method.Get));

            if (!response.IsSuccessful)
            {
                Console.WriteLine(response.Content);
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
                return StatusCode((int)response.StatusCode, errorObj);
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(500, new { message = "Data server returned empty response" });
            }

            var data = JsonConvert.DeserializeObject<DataStructDto>(response.Content);
            return Ok(data);
        }
    }
}