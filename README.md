# asp.net.webapi.framework
This repository is for asp.net web api Framework code


1. Install Cors Package
Microsoft.Owin.Cors
Add below code in startup.cs
```
app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

```

2. Add below code in ConfigureAuth Method
```
public void ConfigureAuth(IAppBuilder app)
{
	PublicClientId = "self";
	OAuthOptions = new OAuthAuthorizationServerOptions
	{
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
	};
}
```


3. Update ApplicationOAuthProvider.css file

```
public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
{
	await Task.Run(() =>
	{
                string userCode = context.UserName;
                string password = context.Password;
                string userName = "Administrator";

                if (userCode == "admin" && password != "")
                {
                    var identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Sid, "12345"));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, "test@gmail.com"));
                    //identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("username", userName);
                    //data.Add("roles", "user");

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
        }

```

