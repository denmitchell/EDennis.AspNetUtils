using EDennis.AspNetUtils;
using EDennis.AspNetUtils.Tests.BlazorSample;
using EDennis.AspNetUtils.Tests.BlazorSample.Services;
using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Radzen;

var builder = WebApplication.CreateBuilder(args);



// Conditionally add support for faking a user, which must be registered
// in AppUser table

#if DEBUG
var fakeUser = builder.Configuration["FakeUser"];
if (fakeUser != null)
    builder.Services.AddFakeAuthentication();
else
{
#endif

    builder.Configuration.AddJsonEnvironmentVariable(
        $"{typeof(Program).Assembly.GetName().Name}.Configuration");

    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

#if DEBUG
}
#endif



//AddAsync CRUD services
builder.AddEntityFrameworkServices<HitsContext>()
    .AddEntityFrameworkService<ArtistService, Artist>() //ArtistService extends EntityFrameworkService to implement cascade delete on Songs
    .AddEntityFrameworkService<Song>(); //there was no need to extend EntityFrameworkService for Song



//Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();



builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();



builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();


//AddAsync simple security based upon AppUser and AppRoles tables
builder.AddSimpleAuthorization<
    EntityFrameworkService<SimpleAuthContext, AppUser>, 
    EntityFrameworkService<SimpleAuthContext,AppRole>>(isBlazorServer: true);



var app = builder.Build();

//ServiceInspector.ServiceDescriptors = builder.Services
//    .OrderBy(s => s.ServiceType.CSharpName())
//    .Select(s => new ServiceDescriptor(s.ServiceType, s.ImplementationType ?? s.ServiceType, s.Lifetime));
//File.WriteAllText("Services.json", JsonSerializer.Serialize(ServiceInspector.ServiceDescriptors.Select(s => new { ServiceType = s.ServiceType.CSharpName(), ImplementationType = s.ImplementationType.CSharpName() }), new JsonSerializerOptions { WriteIndented = true }));
// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
