using ERP_Admin_Panel.Services.Token;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.ProtectedBrowserStorage;
using System.Security.Claims;

namespace ERP_Admin_Panel.Services.StateProviders
{
    public class TokenAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        public TokenAuthStateProvider(ProtectedSessionStorage storage)
        {
            _sessionStorage = storage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //доработать с базой данных
            var token = await _sessionStorage.GetAsync<SecurityToken>(nameof(SecurityToken));

            if (token.Value == null || string.IsNullOrEmpty(token.Value?.AccessToken))
                return CreateAnonymous();

            return CreateUser(token.Value);
        }

        private static AuthenticationState CreateAnonymous()
        {
            var anonymousIdentity = new ClaimsIdentity();
            var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);

            return new AuthenticationState(anonymousPrincipal);
        }

        private static AuthenticationState CreateUser(SecurityToken token)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, token.UserName),
            };

            var identity = new ClaimsIdentity(claims, "Token");
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationState(principal);
        }
    }
}
