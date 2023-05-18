using EDennis.AspNetUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace TestBlazorWasmMsal.Server.Controllers
{
    /// <summary>
    /// Base Controller for retrieving information about the logged-in user.
    /// This is needed for Blazor WASM on the server side.  By default, the
    /// associated <see cref="MsalAccountClaimsPrincipalFactory"/> looks for
    /// a "Me/Info" endpoint; so, setting the subclassed Controller class name
    /// to "MeController" is easiest.
    /// </summary>
    public abstract class UserInfoController : ControllerBase
    {
        private readonly UserNameProvider _userNameProvider;
        public UserInfoController(UserNameProvider userNameProvider)
        {
            _userNameProvider = userNameProvider;
        }

        public string RedirectUri { get; set; }


        [HttpGet]
        public AppUser Info()
        {
            var user = new AppUser
            {
                UserName = User.Identity.IsAuthenticated ? _userNameProvider.UserName : "Anonymous",
                Role = string.Join(',', User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))
            };
            return user;
        }

    }

}
