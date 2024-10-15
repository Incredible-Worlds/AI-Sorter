using AI_Sorter_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Text;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace AI_Sorter_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string ollamaApiUrl = "http://localhost:11434";

        public ProxyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> ProxyGenerate([FromBody] object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ollamaApiUrl + "/api/generate")
            {
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                return Content(responseBody, "application/json");
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"HTTP ошибка: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("serviceStatus")]
        public async Task<string> ProxyStatus()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(ollamaApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    return jsonResponse.response;
                }
                else
                {
                    return $"Ошибка: {response.StatusCode}, {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return $"Исключение: {ex.Message}";
            }
        }
    }
}
