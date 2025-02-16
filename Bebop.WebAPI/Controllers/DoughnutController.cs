using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using WebAPI.Models;
using System.Net.Http;
using WebAPI.Settings;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace WebAPI.Controllers
{
    [Route("doughnuts")]
    [Authorize]
    [ApiController]
    public class DoughnutsController : ControllerBase
    {
        private readonly ApiGateWaySettings _gateWaySettings;
        // Dictionary to store the client API keys
        private readonly HashSet<string> _validApiKeys;
        private readonly HttpClient _roomDesignerServiceClient;
        public DoughnutsController(IHttpClientFactory httpClientFactory,
            IOptions<ApiGateWaySettings> gatewaySettings)
        {
            _gateWaySettings = gatewaySettings.Value;
            // Parse the comma-separated valid API keys from the settings and store them in a HashSet
            _validApiKeys = new HashSet<string>(_gateWaySettings.ValidApiKeys.Split(',').Select(apiKey => apiKey.Trim()));
            _roomDesignerServiceClient = httpClientFactory.CreateClient("RoomDesignerServiceClient");
        }
        //private readonly List<Doughnut> Doughnuts = new List<Doughnut>()
        //{
        //    new Doughnut
        //    {
        //        Name = "Holey Moley",
        //        Filling = "None",
        //        Iced = true,
        //        Price = 1.99
        //    },
        //    new Doughnut
        //    {
        //        Name = "Berry Nice",
        //        Filling = "Raspberry",
        //        Iced = false,
        //        Price = 2.99
        //    },
        //    new Doughnut
        //    {
        //        Name = "Chip Off The Old Choc",
        //        Filling = "Chocolate",
        //        Iced = false,
        //        Price = 2.99
        //    },
        //};

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //if (!CheckKey(out IActionResult actionResult)) return actionResult;
            var response = await _roomDesignerServiceClient.GetAsync("/api/v1/Case");
            return await CheckResponse(response);
        }
        //[HttpPost]
        //public IActionResult Post() 
        //{

        //    return Ok(Doughnuts);
        //}

        private bool IsApiKeyValid(string apiKey)
        {
            // Check if the API key exists in the valid API keys HashSet
            return _validApiKeys.Contains(apiKey);
        }
        private bool CheckKey(out IActionResult actionResult)
        {
            // Check for X-API-Key header
            if (!Request.Headers.TryGetValue("X-API-Key", out var apiKey)) { actionResult = BadRequest("TM-API-Key header is missing."); return false; }
            // Validate the API key
            if (!IsApiKeyValid(apiKey)) { actionResult = actionResult = StatusCode(401, "Invalid API key."); return false; }
            actionResult = StatusCode(200, "API key is valid");
            return true;
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
