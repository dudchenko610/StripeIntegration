using Microsoft.AspNetCore.Identity;
using StripeIntegration.Entities.Interface;

namespace StripeIntegration.Entities.Entities;

public class User : IdentityUser<Guid>, IBaseEntity
{
    public string? Nickname { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string StripeCustomerId { get; set; } = string.Empty;

    public DateTime RegistrationTime { get; set; }
}