using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using WebAPI.Controllers.Extentions;
using WebAPI.Settings;
using System.Linq;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class HabitDiariesController : ControllerBase
    {
        private readonly ApiGateWaySettings _gateWaySettings;
        // Dictionary to store the client API keys
        private readonly HashSet<string> _validApiKeys;
        private readonly HttpClient _diaryServiceClient;
        public HabitDiariesController(IHttpClientFactory httpClientFactory,
            IOptions<ApiGateWaySettings> gatewaySettings)
        {
            _gateWaySettings = gatewaySettings.Value;
            // Parse the comma-separated valid API keys from the settings and store them in a HashSet
            _validApiKeys = new HashSet<string>(_gateWaySettings.ValidApiKeys.Split(',').Select(apiKey => apiKey.Trim()));
            _diaryServiceClient = httpClientFactory.CreateClient("DiaryServiceBaseUrlClient");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _diaryServiceClient.GetAsync("api/HabitDiary/AllDiaries");
            return await CheckResponse(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _diaryServiceClient.GetAsync($"api/HabitDiary/GetDiary/{id}");
            return await CheckResponse(response);
        }


        [HttpGet("GetDiariesByDiaryOwnerId/{id:guid}")]
        public async Task<IActionResult> GetDiariesByDiaryOwnerIdAsync(Guid id)
        {
            var response = await _diaryServiceClient.GetAsync($"api/HabitDiary/GetDiariesByDiaryOwnerId/{id}"); 
            return await CheckResponse(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] JsonElement json)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _diaryServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _diaryServiceClient.PostAsync($"/api/HabitDiary/CreateDiary", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] JsonElement json, Guid id)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _diaryServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _diaryServiceClient.PutAsync($"/api/HabitDiary/UpdateDiary/{id}", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = User.Claims.GenerateJwtToken(_validApiKeys);
            _diaryServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _diaryServiceClient.DeleteAsync($"/api/HabitDiary/DeleteDiary/{id}");
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
