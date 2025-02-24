using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Settings;

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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = GenerateJwtToken(User.Claims);

            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.GetAsync("/api/v1/Cases/owner");
            return await CheckResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JsonElement json)
        {
            var token = GenerateJwtToken(User.Claims);
            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.PostAsync($"/api/v1/Cases", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] JsonElement json)
        {
            var token = GenerateJwtToken(User.Claims);
            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.PutAsync($"/api/v1/Cases", JsonContent.Create(json));
            return await CheckResponse(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = GenerateJwtToken(User.Claims);
            _roomDesignerServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _roomDesignerServiceClient.DeleteAsync($"/api/v1/Cases/{id}");
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

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_validApiKeys.First()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Gateway",
                audience: "Microservices",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
