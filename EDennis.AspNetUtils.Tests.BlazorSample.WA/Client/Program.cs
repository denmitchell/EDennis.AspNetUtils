using EDennis.AspNetUtils;
using EDennis.AspNetUtils.Core.Services.Wasm;
using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;
using EDennis.AspNetUtils.Tests.BlazorSample.WA.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Radzen;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();


#if DEBUG
//retrieve command-line arguments from Server project and determine if there is a FakeUser defined
//if so, set up fake user authorization on the client side.
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://localhost:7244");
Dictionary<string, string> args2 = new();
await Task.Run(async () => {
    args2 = await httpClient.GetFromJsonAsync<Dictionary<string, string>>("Args");
});
    
if(args2.ContainsKey("FakeUser"))
    builder.SetupFakeAuth();
else
    builder.AddMsalAuthentication();
#else 
builder.AddMsalAuthentication();
#endif



builder.AddApiClientServices("EDennis.AspNetUtils.Tests.BlazorSample.WA.ServerAPI")
    .AddApiClientService<AppUser>()
    .AddApiClientService<Artist>()
    .AddApiClientService<Song>();


var host = builder.Build();
await host.RunAsync();

