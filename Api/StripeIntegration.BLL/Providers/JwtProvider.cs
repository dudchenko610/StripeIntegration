using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StripeIntegration.Entities.Entities;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.ViewModels.Options;

namespace StripeIntegration.BLL.Providers;

public class JwtProvider
{
    private readonly IOptions<JwtConnectionOptions> _connectionOptions;
    private readonly UserManager<User> _userManager;

    public JwtProvider(IOptions<JwtConnectionOptions> connectionOptions, UserManager<User> userManager)
    {
        _userManager = userManager;
        _connectionOptions = connectionOptions;
    }

    public async Task<IEnumerable<Claim>> GetUserClaimsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var userRoles = await _userManager.GetRolesAsync(user!);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Sub, user!.Id.ToString()),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName!)
        };

        claims.AddRange(userRoles.Select(item => new Claim(ClaimsIdentity.DefaultRoleClaimType, item)));

        //new ClaimsIdentity(claims,
        //    "Token",
        //    ClaimsIdentity.DefaultNameClaimType,
        //    ClaimsIdentity.DefaultRoleClaimType);

        return claims;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           issuer: _connectionOptions.Value.Issuer,
           audience: _connectionOptions.Value.Audience,
           claims: claims,
           notBefore: DateTime.Now.ToUniversalTime(),
           expires: DateTime.Now.ToUniversalTime().AddMinutes(_connectionOptions.Value.Lifetime),
           signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );
        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return accessToken;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[_connectionOptions.Value.LengthRefreshToken];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_connectionOptions.Value.SecretKey!));
        return symmetricKey;
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetSymmetricSecurityKey(),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        ClaimsPrincipal principal;

        try
        {
            principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        }
        catch
        {
            throw new ServerException("Please input correct data");
        }

        return principal;
    }
}
