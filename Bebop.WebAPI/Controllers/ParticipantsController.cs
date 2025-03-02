using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Controllers.Extentions;
using WebAPI.Settings;

namespace WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly ApiGateWaySettings _gateWaySettings;
        // Dictionary to store the client API keys
        private readonly HashSet<string> _validApiKeys;
        private readonly HttpClient _roomDesignerServiceClient;
        public ParticipantsController(
            IHttpClientFactory httpClientFactory,
            IOptions<ApiGateWaySettings> gatewaySettings)
        {
            _gateWaySettings = gatewaySettings.Value;
            // Parse the comma-separated valid API keys from the settings and store them in a HashSet
            _validApiKeys = new HashSet<string>(_gateWaySettings.ValidApiKeys.Split(',').Select(apiKey => apiKey.Trim()));
            _roomDesignerServiceClient = httpClientFactory.CreateClient("RoomDesignerServiceClient");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = User.Claims.GenerateJwtToken(_validApiKeys);

            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.GetAsync("/api/v1/Participants/owner");
            return await CheckResponse(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = User.Claims.GenerateJwtToken(_validApiKeys);

            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.GetAsync($"/api/v1/Participants/check/{id}");
            return await CheckResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JsonElement json)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.PostAsync($"/api/v1/Participants", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.DeleteAsync($"/api/v1/Participants/{id}");
            return await CheckResponse(response);
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
