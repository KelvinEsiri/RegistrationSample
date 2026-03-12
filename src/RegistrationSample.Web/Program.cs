using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RegistrationSample.Web;
using RegistrationSample.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5257";

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped(sp =>
{
    var tokenService = sp.GetRequiredService<TokenService>();
    var handler = new ApiAuthorizationMessageHandler(tokenService)
    {
        InnerHandler = new HttpClientHandler()
    };
    return new HttpClient(handler) { BaseAddress = new Uri(apiBase) };
});

await builder.Build().RunAsync();
