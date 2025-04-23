using Application.DTOs.CheckoutDTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Application.Services;

public class StripeService : IStripeService
{
    private readonly ILogger _logger;

    public StripeService(ILogger<StripeService> logger)
    {
        _logger = logger;
    }


    public Task<Customer> CreateCustomerAsync(string email, string name, string phone)
    {
        throw new NotImplementedException();
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(CardInfo card)
    {
        var options = new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions
            {
                Number = card.CardNumber,
                ExpMonth = card.ExpMonth,
                ExpYear = card.ExpYear,
                Cvc = card.CVV
            }
        };

        var service = new PaymentMethodService();
        return await service.CreateAsync(options);
    }

    public Task<PaymentIntent> ConfirmPaymentIntentAsync(string customerId, long amount, string currency,
        string paymentMethodId)
    {
        throw new NotImplementedException();
    }

    public async Task<Session> CreateCheckoutSessionAsync(
        CheckoutSessionRequestDto request,
        decimal shippingCost,
        string shippingLabel,
        string successUrl,
        string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            CustomerEmail = request.CustomerEmail,
            LineItems = request.LineItems
                .Select(li => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = li.PriceData.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = li.PriceData.ProductData.Name,
                            Metadata = li.PriceData.ProductData.Metadata
                        },
                        UnitAmount = li.PriceData.UnitAmount
                    },
                    Quantity = li.Quantity
                })
                .Append(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = shippingLabel
                        },
                        UnitAmount = (long)(shippingCost * 100)
                    },
                    Quantity = 1
                })
                .ToList(),
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "phone", request.CustomerPhone }
                }
            }
        };

        var service = new SessionService();
        return await service.CreateAsync(options);
    }

    public Event ConstructEvent(string json, string signature, string secret)
    {
        return EventUtility.ConstructEvent(json, signature, secret);
    }

    public async Task<Session> GetSessionAsync(string sessionId)
    {
        var service = new SessionService();
        return await service.GetAsync(sessionId);
    }
}