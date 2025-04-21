using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using System.Text.Json.Serialization;

namespace AI_Sorter.Services;

public class AuthService
{
	private readonly HttpClient _http;
	private readonly ILocalStorageService _localStorage;
	private readonly CustomAuthStateProvider _customAuthStateProvider;

	public AuthService(HttpClient http, ILocalStorageService localStorage, CustomAuthStateProvider customAuthStateProvider	)
	{
		_http = http;
		_localStorage = localStorage;
		_customAuthStateProvider = customAuthStateProvider;
	}

	public async Task<bool> Login(string login, string password)
	{
		var response = await _http.PostAsJsonAsync("api/auth/login", new { login, password });
		if (!response.IsSuccessStatusCode) return false;

		var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
		await _localStorage.SetItemAsync("authToken", result!.Token);

		return true;
	}

	public async Task LogoutAsync()
	{
		await _localStorage.RemoveItemAsync("authToken");
		_customAuthStateProvider.NotifyUserLogout();
	}

	public async Task<string?> GetToken() => await _localStorage.GetItemAsync<string>("authToken");
}

public class TokenResponse
{
	public string Token { get; set; } = "";
}
