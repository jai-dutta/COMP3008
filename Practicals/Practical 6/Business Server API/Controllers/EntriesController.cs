using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Business_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly RestClient _client;

        public EntriesController()
        {
            _client = new RestClient("http://localhost:5050/api/Database/");
        }

        // GET api/entries
        [HttpGet]
        public IActionResult GetNumEntries()
        {
            var response = _client.Execute(new RestRequest("", Method.Get));
            if (!response.IsSuccessful)
                return StatusCode((int)response.StatusCode, response.Content);

            var numEntries = JsonConvert.DeserializeObject<int>(response.Content!);
            return Ok(numEntries);
        }
    }
}