using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using robot_controller_api.Models;
using BCrypt;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserAccess _userRepo;
    public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserAccess userDataAccess
        )
            : base(options, logger, encoder, clock)
        {
            _userRepo = userDataAccess;
        }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        base.Response.Headers.Append("WWW-Authenticate", @"Basic realm=""Access to the robot controller.""");
        var authHeader = base.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
        {
            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail($"Invalid authentication header."));
        }

        var base64Str = authHeader.Substring("Basic ".Length).Trim();


        try
        {
            var decodedBytes = Convert.FromBase64String(base64Str);
            var credentials = Encoding.UTF8.GetString(decodedBytes).Split(':');

            var email = credentials[0];
            var password = credentials[1];

            var user = _userRepo.GetUsers().FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail($"user not found."));
            }

            var hasher = BCrypt.Net.BCrypt.HashPassword(password);
            var pwVerificationResult = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (pwVerificationResult)
            {
                var claims = new[]
                {
                    new Claim("name", $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role!)
                };

                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(authTicket));
            }
            else
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail($"Authentication not passed."));
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail("Failed: " + ex.Message));
        }
    }

}