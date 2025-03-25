using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Controllers.Extentions;
using WebAPI.Settings;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class HabitsController : ControllerBase
    {
        private readonly ApiGateWaySettings _gateWaySettings;
        // Dictionary to store the client API keys
        private readonly HashSet<string> _validApiKeys;
        private readonly HttpClient _habitsServiceClient;
        public HabitsController(IHttpClientFactory httpClientFactory,
            IOptions<ApiGateWaySettings> gatewaySettings)
        {
            _gateWaySettings = gatewaySettings.Value;
            // Parse the comma-separated valid API keys from the settings and store them in a HashSet
            _validApiKeys = new HashSet<string>(_gateWaySettings.ValidApiKeys.Split(',').Select(apiKey => apiKey.Trim()));
            _habitsServiceClient = httpClientFactory.CreateClient("BookOfHabitsServiceClient");
        }

        [HttpGet("room/person/{id:guid}")]
        public async Task<IActionResult> Get(Guid id, [FromQuery] Guid personId)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var actionUrl = $"/api/v1/Habits/room/person/{id}";
            var response = await _habitsServiceClient.GetAsync($"{actionUrl}?{nameof(personId)}={personId}");
            return await CheckResponse(response);
        }

        [HttpGet("room/{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _habitsServiceClient.GetAsync($"/api/v1/Habits/room/{id}");
            return await CheckResponse(response);
        }

        [HttpGet("rooms/{id:guid}")]
        public async Task<IActionResult> GetRoomById(Guid id)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _habitsServiceClient.GetAsync($"/api/v1/Rooms/{id}");
            return await CheckResponse(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _habitsServiceClient.GetAsync($"/api/v1/Habits/{id}");
            return await CheckResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JsonElement json)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _habitsServiceClient.PostAsync($"/api/v1/Habits", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] JsonElement json)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _habitsServiceClient.PutAsync($"/api/v1/Habits", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        /// <summary>
        /// Обновление подписей.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSignatures(Guid id, [FromBody] JsonElement json)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _habitsServiceClient.PutAsync($"/api/v1/Habits/{id}", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _habitsServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _habitsServiceClient.DeleteAsync($"/api/v1/Habits/{id}");
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
