using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApiTemplate.Seguranca;

[assembly: OwinStartup(typeof(WebApiTemplate.App_Start.Startup))]

namespace WebApiTemplate.App_Start
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            int time = 0;
            string accessTokenExpireTimeSpan = ConfigurationManager.AppSettings["AccessTokenExpireTimeSpan"];
            if (!int.TryParse(accessTokenExpireTimeSpan, out time))
                time = 60;

            //IAcessoService acessoService= WebApiApplication.kernel.Get<IAcessoService>();
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/acesso/token"),
                Provider = new OAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(time),
                AllowInsecureHttp = true
            };
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }

}