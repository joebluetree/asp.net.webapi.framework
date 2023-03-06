# Token Generation

1. Install Cors Package<br/>
Microsoft.Owin.Cors<br/>
then add below code in startup.cs<br/>

```
app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

```

2. Update below code in startup.auth.cs file
```
using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApp.Providers;

namespace WebApp
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            ///app.CreatePerOwinContext(ApplicationDbContext.Create);
            ///app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            ///app.UseCookieAuthentication(new CookieAuthenticationOptions());
            ///app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                ///AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}
```


3. Changes in ApplicationOAuthProvider.cs file

```
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
namespace WebApp.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ///var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ///ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            await Task.Run(() =>
            {

                var userName = context.UserName;
                var password = context.Password;
                if (userName == "admin" && password == "admin")
                {
                    var identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Sid, "283938393"));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, userName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("username", userName);
                    data.Add("roles", "user");

                    var properties = new AuthenticationProperties(data);
                    var ticket = new AuthenticationTicket(identity, properties);
                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            });

            /*
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);
            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            */
            ///context.Validated(ticket);
            ///context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}
```



4. Now add a class under controller <br/>
Then add below code to setup web api methods

```
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UserAdmin.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("showmessage")]
        public IHttpActionResult ShowMessage()
        {
            try
            {
                //throw new Exception("Error");
               return Ok("Hello");
            }
            catch ( Exception Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }
        [HttpGet]
        [Route("showmessage2")]
        public IHttpActionResult showmessage2( [FromUri] string msg,  [FromUri] string msg2)
        {
            try
            {
                //throw new Exception("Error");
                return Ok(msg + " " + msg2);
            }
            catch (Exception Ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message.ToString()));
            }
        }

    }
}
```
