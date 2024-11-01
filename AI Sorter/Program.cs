using AI_Sorter;
using AI_Sorter.Services;
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

await builder.Build().RunAsync();
