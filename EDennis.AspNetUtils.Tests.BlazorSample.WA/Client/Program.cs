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

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();


builder.SetupFakeAuth();
//builder.AddMsalAuthentication();


builder.AddApiClientServices("EDennis.AspNetUtils.Tests.BlazorSample.WA.ServerAPI")
    .AddApiClientService<AppUser>()
    .AddApiClientService<Artist>()
    .AddApiClientService<Song>();


var host = builder.Build();
await host.RunAsync();

