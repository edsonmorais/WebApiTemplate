using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace WebApiTemplate.Seguranca
{
    public class Security
    {
        public static string GetKeyUser()
        {
            Claim claim = ((ClaimsPrincipal)HttpContext.Current.User).Claims.FirstOrDefault(f => f.Type == ClaimTypes.Sid);

            return claim?.Value;
        }
    }
}