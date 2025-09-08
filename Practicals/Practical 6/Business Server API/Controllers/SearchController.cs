using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using DTOS;

namespace Business_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly RestClient _client;

        public SearchController()
        {
            _client = new RestClient("http://localhost:5050/api/Database/");
        }

        // POST api/search
        [HttpPost]
        public IActionResult Search([FromBody] SearchQueryDto search)
        {
            var response = _client.Execute(new RestRequest("all", Method.Get));

            if (!response.IsSuccessful)
                return StatusCode((int)response.StatusCode, response.Content);

            var allRecords = JsonConvert.DeserializeObject<List<DataStructDto>>(response.Content!);

            var match = allRecords!.Find(d =>
                d.lname.Equals(search.searchQuery, StringComparison.OrdinalIgnoreCase));

            if (match == null)
            {
                return NotFound(new { Message = "Last name not found" });
            }

            return Ok(match);
        }
    }
}