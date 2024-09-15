namespace StripeIntegration.ViewModels.Models.Products;

public class ProductModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImgName { get; set; }

    public string? Currency { get; set; }
    public string? PriceId { get; set; }
    public decimal Price { get; set; }
}