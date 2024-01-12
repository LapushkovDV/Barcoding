using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ERP_API_Service.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        public int ExpireMinutes { get; set; } = 10080;
        public string SecretKey { get; set; } = "TW9zaGVFcmV6UHJpdmF0ZUtleQ==";
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public Claim[] Claims { get; set; }
    }
}