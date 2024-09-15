namespace StripeIntegration.ViewModels.Options;

public class JwtConnectionOptions
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? SecretKey { get; set; }
    public int Lifetime { get; set; }
    public int LengthRefreshToken { get; set; }
}
