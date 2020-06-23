using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using app_nehmen_api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace app_nehmen_api.Middleware
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        UserConfig _userConfig;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            IOptions<UserConfig> userConfig,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _userConfig = userConfig.Value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Console.WriteLine("Auth begins");
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            Console.WriteLine("Header present");
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];

                if (username != _userConfig.Username || password != _userConfig.Password)
                {
                    Console.WriteLine("Username or password incorrect");
                    return AuthenticateResult.Fail("Username or password incorrect");
                }
            }
            catch
            {
                Console.WriteLine("Invalid Authorization Header");
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, "ApiUser")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            Console.WriteLine("Auth ends successfully");
            return AuthenticateResult.Success(ticket);
        }
    }
}