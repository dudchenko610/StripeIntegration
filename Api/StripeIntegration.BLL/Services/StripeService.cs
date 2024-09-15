using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StripeIntegration.BLL.Services.Interfaces;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.ViewModels.Models.Subscriptions;
using StripeIntegration.ViewModels.Options;

namespace StripeIntegration.BLL.Services;

public class StripeService : IStripeService
{
    private readonly EventService _eventService;
    private readonly RefundService _refundService;
    private readonly InvoiceService _invoiceService;
    private readonly Stripe.ProductService _productService;
    private readonly Stripe.Checkout.SessionService _sessionService;
    private readonly CustomerService _customerService;
    private readonly Stripe.SubscriptionService _subscriptionService;
    private readonly PriceService _priceService;

    private readonly ClientConnectionOptions _clientOptions;

    public StripeService(
        EventService eventService,
        RefundService refundService,
        InvoiceService invoiceService,
        Stripe.ProductService productService,
        Stripe.Checkout.SessionService sessionService,
        CustomerService customerService,
        Stripe.SubscriptionService subscriptionService,
        IOptions<ClientConnectionOptions> clientOptions,
        PriceService priceService)
    {
        _eventService = eventService;
        _refundService = refundService;
        _invoiceService = invoiceService;
        _productService = productService;
        _sessionService = sessionService;
        _customerService = customerService;
        _subscriptionService = subscriptionService;
        _priceService = priceService;

        _clientOptions = clientOptions.Value;
    }

    public async Task<string> CreateCustomerAsync(string email)
    {
        var customer = await _customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Name = email,
        });

        return customer.Id;
    }

    public async Task<StripeList<Product>> GetProductsAsync()
    {
        var options = new ProductListOptions { Limit = 500 };
        options.AddExpand("data.default_price");

        var service = new Stripe.ProductService();
        var stripeProducts = await service.ListAsync(options);

        return stripeProducts;
    }

    public async Task<PaymentSessionModel> GetPaymentSessionAsync(string customerId, string priceId)
    {
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>()
            {
                new()
                {
                    Price = priceId,
                    Quantity = 1,
                },
            },
            Mode = "subscription",
            AllowPromotionCodes = false,
            Customer = customerId,
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            CancelUrl = $"{_clientOptions.Url}/profile",
            SuccessUrl = $"{_clientOptions.Url}/payment-success/{{CHECKOUT_SESSION_ID}}",
        };

        var session = await _sessionService.CreateAsync(options);

        var paymentSessionLink = new PaymentSessionModel
        {
            Id = session.Id,
            Url = session.Url
        };

        return paymentSessionLink;
    }

    public async Task<List<Subscription>> GetCustomerSubscriptionsAsync(string customerId)
    {
        var options = new SubscriptionListOptions
        {
            Limit = 500,
            Customer = customerId,
        };

        //options.AddExpand("data.items.data.price.product");

        var subscriptions = await _subscriptionService.ListAsync(options);

        return subscriptions.ToList();
    }

    public Task<Subscription> GetSubscriptionAsync(string subscriptionId)
    {
        var options = new SubscriptionGetOptions();
        options.AddExpand("items.data.price.product");

        var subscription = _subscriptionService.GetAsync(subscriptionId, options);

        return subscription;
    }

    public async Task CancelSubscriptionAsync(string subscriptionId)
    {
        try
        {
            var subscription = await _subscriptionService.CancelAsync(subscriptionId);

            if (subscription?.Status != "canceled")
                throw new ServerException("Unable to cancel current Subscription");
        }
        catch (Exception e)
        {
            throw new ServerException("Unable to cancel current Subscription");
        }
    }


    public async Task ChangeSubscriptionAsync(string subscriptionId, string newPriceId, string subscriptionItemId)
    {
        var options = new SubscriptionUpdateOptions
        {
            CancelAtPeriodEnd = false,
            ProrationBehavior = "always_invoice",

            Items = new List<SubscriptionItemOptions>
            {
                new()
                {
                    Id = subscriptionItemId,
                    Price = newPriceId,
                },
            }
        };

        try
        {
            await _subscriptionService.UpdateAsync(subscriptionId, options);
        }
        catch (Exception e)
        {
            throw new ServerException("Unable to change subscription");
        }
    }

    public async Task<Product?> GetProductAsync(string productId)
    {
        var product = await _productService.GetAsync(productId);
        return product;
    }

    public async Task<Product?> GetProductByPriceIdAsync(string priceId)
    {
        var options = new PriceGetOptions();
        options.AddExpand("product");

        var price = await _priceService.GetAsync(priceId, options);
        return price?.Product;
    }
}