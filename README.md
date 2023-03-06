# DotNet Framework Web Api Project 
This repository is for asp.net web api Framework Project

<a href="./startup.md">Token Generation </a>
<a href="./dal.md">Data Access Layer </a>





1. Install Cors Package<br/>
Microsoft.Owin.Cors<br/>
then add below code in startup.cs<br/>

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
