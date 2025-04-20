using AI_Sorter;
using AI_Sorter.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<OllamaApiService>();
builder.Services.AddSingleton<UploadFile>();
builder.Services.AddSingleton<ApiServices>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped(sp =>
	new HttpClient { BaseAddress = new Uri("http://ai-sortme.local/api/") });
builder.Services.AddBlazoredLocalStorage();


await builder.Build().RunAsync();
