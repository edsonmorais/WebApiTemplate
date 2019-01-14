using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebApiTemplate.App_Start;

namespace WebApiTemplate.Seguranca
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        public OAuthProvider()
        {
        }
        public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            return base.ValidateTokenRequest(context);
        }




        public override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {

                    string token = context.Parameters["Token"];
                    bool tokenOk = true; //to do: Method which have to validate the token

                    if (tokenOk)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Sid, Convert.ToString(token)),
                        };

                        ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);

                        var properties = CreateProperties(token);
                        var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                        context.Validated(ticket);
                    }
                    else
                    {
                        context.SetError("invalid_grant", "Autenticação de usuario invalida.");
                    }
                }
                catch (Exception ex)
                {
                    context.SetError("invalid_grant", "Autenticação de usuario invalida.");
                }
                finally
                {

                }
            });
        }

        #region[GrantResourceOwnerCredentials]
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                bool userOk = true;
                var userName = context.UserName;
                var password = context.Password;
                //var userService = new UserService(); // our created one
                //var user = userService.ValidateUser(userName, password);
                if (userOk)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Sid, Convert.ToString(userName)),
                        new Claim(ClaimTypes.Name, userName)
                    };
                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims,
                                Startup.OAuthOptions.AuthenticationType);

                    var properties = CreateProperties(userName);
                    var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect");
                }
            });
        }
        #endregion

        #region[ValidateClientAuthentication]
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }
        #endregion

        #region[TokenEndpoint]
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        #endregion

        #region[CreateProperties]
        public static AuthenticationProperties CreateProperties(string token)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "token", token }
            };
            return new AuthenticationProperties(data);
        }
        #endregion
    }
}