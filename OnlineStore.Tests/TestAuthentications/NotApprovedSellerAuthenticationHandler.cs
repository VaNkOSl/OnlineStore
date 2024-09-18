﻿namespace OnlineStore.Tests.TestAuthentications;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using static DataBaseSeeder;
public class NotApprovedSellerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public NotApprovedSellerAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier,NotApprovedSellerUser!.Id.ToString()),
            new Claim(ClaimTypes.Name, "test"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
