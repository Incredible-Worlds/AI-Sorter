using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AI_Sorter.Services
{
    public class OllamaApiService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiUrl = "http://37.193.240.201:11445/api/Proxy/generate";

        public async Task<string> GetOllamaResponseAsync(string prompt, string modelName)
        {
            var requestData = new
            {
                model = modelName,  
                prompt = prompt,
                stream = false
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

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
