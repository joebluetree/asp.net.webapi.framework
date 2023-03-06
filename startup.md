# Token Generation

## 1. Install Cors Package<br/>
Microsoft.Owin.Cors<br/>
then add below code in startup.cs<br/>

```
app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

```

## 2. Update below code in startup.auth.cs file
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
		//RefreshTokenProvider = new OAuthCustomRefreshTokenProvider(),
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

	public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
	{
		var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
		var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
		context.Validated(newTicket);
		return Task.FromResult<object>(null);
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



4. Add refresh token provider<br/>

OAuthCustomRefreshTokenProvider.cs


```
using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Collections.Concurrent;
using Microsoft.Owin.Security.Infrastructure;

namespace WebApp.Providers
{
    public class OAuthCustomRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();
            /* Copy claims from previous token
             ***********************************/
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(40)
            };
            var refreshTokenTicket = await Task.Run(() => new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties));

            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle  
            context.SetToken(guid);
        }
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            string header = await Task.Run(() => context.OwinContext.Request.Headers["Authorization"]);

            if (_refreshTokens.TryRemove(context.Token, out ticket))
                context.SetTicket(ticket);
        }
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}
```
