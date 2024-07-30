using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthCustomDb.Token;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public TokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }


    public TokenDetails BuildToken(TokenGeneratorClaims tokenGeneratorClaims)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, tokenGeneratorClaims.Email),
            new Claim(ClaimTypes.Name, tokenGeneratorClaims.Email),
            new Claim(ClaimTypes.NameIdentifier, tokenGeneratorClaims.UserId.ToString())
        }.Concat(tokenGeneratorClaims.Roles.Select(e => new Claim(ClaimTypes.Role, e.ToString())));
        var expiryInMins = _jwtSettings.ExpiryMins ?? 60;

        return WriteToken(claims, expiryInMins, _jwtSettings.Key, _jwtSettings.Issuer, _jwtSettings.Audience);
    }

    private static TokenDetails WriteToken(IEnumerable<Claim> claims, int expiryMins, string key, string issuer, string audience)
    {
        var expiresAt = DateTime.Now.AddMinutes(expiryMins);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
            expires: expiresAt, signingCredentials: credentials);

        return new TokenDetails(new JwtSecurityTokenHandler().WriteToken(tokenDescriptor), expiresAt);
    }
}