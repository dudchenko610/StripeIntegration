namespace StripeIntegration.Website.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class UnauthorizedAttribute : Attribute
{
}