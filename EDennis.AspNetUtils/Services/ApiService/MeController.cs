using EDennis.AspNetUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace TestBlazorWasmMsal.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public abstract class MeControllerBase : ControllerBase
    {
        private readonly UserNameProvider _userNameProvider;
        public MeControllerBase(UserNameProvider userNameProvider)
        {
            _userNameProvider = userNameProvider;
        }

        public string RedirectUri { get; set; }


        [HttpGet]
        public AppUser Info()
        {
            return new AppUser
            {
                UserName = User.Identity.IsAuthenticated ? _userNameProvider.UserName : "Anonymous",
                Role = string.Join(',', User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))
            };
        }

    }

}
