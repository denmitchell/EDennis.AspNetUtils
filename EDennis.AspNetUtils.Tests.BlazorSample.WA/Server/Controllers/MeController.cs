using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using TestBlazorWasmMsal.Server.Controllers;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class MeController : UserInfoController
    {
        public MeController(UserNameProvider userNameProvider) : base(userNameProvider)
        {
        }
    }
}
