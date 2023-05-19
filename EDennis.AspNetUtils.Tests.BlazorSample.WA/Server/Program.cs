using EDennis.AspNetUtils;
using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;
using EDennis.AspNetUtils.Tests.BlazorSample.WA.Server;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Radzen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("https://login.microsoft.com","https://localhost:7244"); // add the allowed origins  
                      });
});

#if DEBUG
var fakeUser = builder.Configuration["FakeUser"];
if (fakeUser != null)
    builder.Services.AddFakeAuthentication();
else
{
#endif

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

#if DEBUG
}
#endif

//Add Simple Authorization
builder.AddSimpleAuthorization<
    EntityFrameworkService<SimpleAuthContext, AppUser>,
    EntityFrameworkService<SimpleAuthContext, AppRole>> (false);


//Add CRUD services
builder.AddEntityFrameworkServices<HitsContext>()
    .AddEntityFrameworkService<ArtistService, Artist>() //ArtistService extends EntityFrameworkService to implement cascade delete on Songs
    .AddEntityFrameworkService<Song>(); //there was no need to extend EntityFrameworkService for Song


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
//app.UseCors("MyAllowSpecificOrigins");

app.Use(async (context, next) =>
{
    System.Diagnostics.Debug.WriteLine("PRE AUTHORIZATION");
    var claims = string.Join(',',context.User.Claims.Select(c => $"{c.Type}={c.Value}").ToArray());
    System.Diagnostics.Debug.WriteLine(claims);
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseSimpleAuthorization();

app.Use(async (context, next) =>
{
    System.Diagnostics.Debug.WriteLine("POST AUTHORIZATION");
    var claims = string.Join(',', context.User.Claims.Select(c => $"{c.Type}={c.Value}").ToArray());
    System.Diagnostics.Debug.WriteLine(claims);
    await next();
});

app.MapRazorPages();
app.MapControllers();

//add _Host.cshtml file from standard Blazor Server project, updating the rendering mode to Server
// and pointing the body component type to the wasm client's App component
//app.MapFallbackToPage("/_Host");
app.MapFallbackToFile("index.html");

app.Run();
