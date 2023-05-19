using EDennis.AspNetUtils;
using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;
using EDennis.AspNetUtils.Tests.BlazorSample.WA.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();


//builder.Services.AddMsalAuthentication(options =>
//{
//    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
//    options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration.GetSection("ServerApi")["Scopes"]);
//});

builder.AddMsalAuthentication();

builder.AddApiClientServices("EDennis.AspNetUtils.Tests.BlazorSample.WA.ServerAPI")
    .AddApiClientService<AppUser>()
    .AddApiClientService<Artist>()
    .AddApiClientService<Song>();




var host = builder.Build();
await host.RunAsync();





/*
 //OLD
builder.Services.AddHttpClient("EDennis.AspNetUtils.Tests.BlazorSample.WA.ServerAPI", 
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("EDennis.AspNetUtils.Tests.BlazorSample.WA.ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration.GetSection("ServerApi")["Scopes"]);
});
*/

