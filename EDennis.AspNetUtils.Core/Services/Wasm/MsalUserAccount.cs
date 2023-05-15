using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Dummy implementation of <see cref="RemoteUserAccount"/> for use with
    /// <see cref="MsalAccountClaimsPrincipalFactory"/>
    /// </summary>
    public class MsalUserAccount : RemoteUserAccount
    {
    }
}
