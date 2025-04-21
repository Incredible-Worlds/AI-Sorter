using System.Net.Http.Json;

namespace AI_Sorter.Services
{
	public class LogService
	{
		private readonly HttpClient _httpClient;

		public LogService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> GetLogs(int lines = 100)
		{
			var response = await _httpClient.GetFromJsonAsync<ApiResponse>($"log/get-logs?lines={lines}");
			return response?.Logs ?? "No logs available";
		}

		public class ApiResponse
		{
			public string Logs { get; set; }
		}
	}
}
