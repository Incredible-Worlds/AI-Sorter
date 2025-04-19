namespace AI_Sorter;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _localStorage;

	public CustomAuthStateProvider(ILocalStorageService localStorage)
	{
		_localStorage = localStorage;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var token = await _localStorage.GetItemAsync<string>("authToken");

		var identity = string.IsNullOrWhiteSpace(token)
			? new ClaimsIdentity()
			: new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

		var user = new ClaimsPrincipal(identity);
		return new AuthenticationState(user);
	}

	public void NotifyUserAuthentication(string token)
	{
		var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
		var user = new ClaimsPrincipal(identity);

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
	}

	public void NotifyUserLogout()
	{
		var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
	}

	private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
	{
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(jwt);
		return token.Claims;
	}
}

