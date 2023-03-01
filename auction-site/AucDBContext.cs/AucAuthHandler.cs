using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

public class AucAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IAucRepo _repository;

    public AucAuthHandler(
        IAucRepo repository,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _repository = repository;
    }
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            Response.Headers.Add("www-authenticate", "Basic");
            return AuthenticateResult.Fail("Authorization header not found.");
        }
        else
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
            var username = credentials[0];
            var password = credentials[1];

            if (_repository.ValidLoginAdmin(username, password) || (_repository.ValidLogin(username, password) && _repository.ValidLoginAdmin(username, password)) )
            {
                var claims = new[] { new Claim("admin", username) };

                ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
             
                
                return AuthenticateResult.Success(ticket);
            }
            else if (_repository.ValidLogin(username, password) && !_repository.ValidLoginAdmin(username, password) )
            {
                var claims = new[] { new Claim("UserName", username) };

                ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            Response.Headers.Add("www-authenticate", "Basic");
            return AuthenticateResult.Fail("userName and password do not match or not belong to admin");
        }
    }
}