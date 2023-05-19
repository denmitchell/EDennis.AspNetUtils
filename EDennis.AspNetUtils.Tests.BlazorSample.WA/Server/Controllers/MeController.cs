using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using TestBlazorWasmMsal.Server.Controllers;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class MeController : UserInfoController
    {

        public MeController(UserNameProvider userNameProvider,
            IConfiguration config) : base(userNameProvider, config)
        {
        }
    }
}
