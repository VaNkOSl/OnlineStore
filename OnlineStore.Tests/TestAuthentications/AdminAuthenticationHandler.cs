namespace OnlineStore.Tests.TestAuthentications;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using static DataBaseSeeder;
public class AdminAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public AdminAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier,AdminUser!.Id.ToString()),
            new Claim(ClaimTypes.Name, "Admin"),
            new Claim(ClaimTypes.Email, "admin@adminTest.com"),
            new Claim(ClaimTypes.Role, "Administrator")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
