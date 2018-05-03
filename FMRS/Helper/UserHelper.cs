using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
namespace FMRS.Helper
{

    public static class UserHelper 
    {
        public static string UserInfo(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);
            return (claim != null) ? claim.Value : string.Empty;
        }

    }
}
