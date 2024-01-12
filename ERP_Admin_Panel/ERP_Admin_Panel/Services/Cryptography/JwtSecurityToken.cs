using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ERP_Admin_Panel.Services.Cryptography
{
    public static class JwtTokenSecurity
    {
        private static readonly SymmetricSecurityKey SecurityKey = new(System.Text.Encoding.UTF8.GetBytes("G@l@Kt|Ka_T$d+enabled"));
        private static readonly SigningCredentials Credentials;

        static JwtTokenSecurity()
        {
            Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        }
        public static string CreateJwt(string userName, int expireMinutes = 0)
        {
            var claims = new[]
			{
                new Claim(ClaimTypes.Name, userName),
				new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Email, userName),
                new Claim(JwtRegisteredClaimNames.Jti, userName)
			};

            var token = new JwtSecurityToken(signingCredentials:Credentials, expires: expireMinutes == 0 ? null : DateTime.Now.AddMinutes(expireMinutes), claims: claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
