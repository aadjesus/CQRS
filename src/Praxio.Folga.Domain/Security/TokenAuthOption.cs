using Microsoft.IdentityModel.Tokens;
using System;

namespace Praxio.Folga.Domain.Security
{
    public class TokenAuthOption
    {
        public static string Audience { get; } = "FolgaAudience";
        public static string Issuer { get; } = "FolgaIssuer";
        public static RsaSecurityKey Key { get; } = new RsaSecurityKey(RSAKeyHelper.GenerateKey());
        public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromDays(7);
    }
}
