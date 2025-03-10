using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI.Settings;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly ApiGateWaySettings _gateWaySettings;
        // Dictionary to store the client API keys
        private readonly HashSet<string> _validApiKeys;
        private readonly HttpClient _cardsServiceClient;
        public CardsController(IHttpClientFactory httpClientFactory,
            IOptions<ApiGateWaySettings> gatewaySettings)
        {
            _gateWaySettings = gatewaySettings.Value;
            // Parse the comma-separated valid API keys from the settings and store them in a HashSet
            _validApiKeys = new HashSet<string>(_gateWaySettings.ValidApiKeys.Split(',').Select(apiKey => apiKey.Trim()));
            _cardsServiceClient = httpClientFactory.CreateClient("BookOfHabitsServiceClient");
        }


        private async Task<IActionResult> CheckResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            else return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }
    }
}
