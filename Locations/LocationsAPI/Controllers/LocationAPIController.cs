using LocationsAPI.Data;
using LocationsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.Json;

namespace LocationsAPI.Controllers
{
    [Route("api/LocationsAPI")]
    [ApiController]
    public class LocationAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<LocationHub> _hubContext;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public LocationAPIController(ApplicationDbContext db, IHubContext<LocationHub> hubContext, IConfiguration configuration)
        {
            _db = db;
            _hubContext = hubContext;
            _rabbitMQPublisher = new RabbitMQPublisher(configuration);
        }

        [HttpGet("Requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<LocationRequest>> GetRequests()
        {
            return Ok(_db.Requests);
        }

        [HttpGet("Responses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<LocationResponse>> GetResponses()
        {
            return Ok(_db.Responses);
        }

        [HttpGet("Request/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LocationRequest> GetRequest(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var req = _db.Requests.FirstOrDefault(u => u.Id == id);
            if (req == null)
            {
                return NotFound();
            }
            return Ok(req);
        }

        [HttpGet("Response/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LocationResponse> GetResponse(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var res = _db.Responses.FirstOrDefault(u => u.Id == id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLocationsInRadius([FromBody] LocationRequest locationReq)
        {
            string apiKey = "API_KEY";
            string apiUrl = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={locationReq.Latitude},{locationReq.Longitude}&radius={locationReq.Radius}&type={locationReq.Category}&key={apiKey}";
            var httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if(locationReq.Id > 0)
            {
                return BadRequest();
            }

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonDocument.Parse(jsonResponse);
                var results = jsonObject.RootElement.GetProperty("results");

                _db.Requests.Add(locationReq);
                _db.SaveChanges();

                LocationResponse locationResp = new()
                {   
                    StatusCode = response.StatusCode,
                    Result = jsonResponse,
                    ReqId = locationReq.Id
                };

                _db.Responses.Add(locationResp);
                _db.SaveChanges();

                await _hubContext.Clients.All.SendAsync("ReceiveRequest", locationReq);
                _rabbitMQPublisher.PublishRequest(JsonSerializer.Serialize(locationReq));

                return Ok(results);
            }
            else
            {
                return BadRequest("Request failed with status code: " + response.StatusCode);
            }
        }
    }
}
