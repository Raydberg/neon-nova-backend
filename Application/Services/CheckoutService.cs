using Application.DTOs.CheckoutDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;

namespace Application.Services;

public class CheckoutService : ICheckoutService
{
    private readonly ICheckoutRepository _repository;
    private readonly IStripeService _stripeService;
    private readonly ILogger<CheckoutService> _logger;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CheckoutService(ICheckoutRepository repository, IStripeService stripeService,
        ILogger<CheckoutService> logger, IMapper mapper, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _stripeService = stripeService;
        _logger = logger;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<int> SavePersonalInfoAsync(PersonalInfoDto dto)
    {
        var user = await _currentUserService.GetUser();
        var userId = user!.Id;
        _logger.LogInformation("Guardando dirección de envío para el usuario {UserId}", userId);

        var address = new Address
        {
            UserId = userId,
            Street = dto.Address.Street,
            City = dto.Address.City,
            PostalCode = dto.Address.PostalCode,
            ShippingCost = dto.ShippingCost,
            IsPrimary = false
        };

        var savedAddress = await _repository.SaveShippingAddressAsync(address);
        return savedAddress.Id;
    }

    public async Task<string> CreatePaymentMethodAsync(PaymentMethodDto dto)
    {
        if (dto.PaymentMethod.ToLower() != "card")
            throw new ApplicationException("Solo se acepta el método de pago con tarjeta.");

        var card = new CardInfo
        {
            CardHolder = dto.Card.CardHolder,
            CardNumber = dto.Card.CardNumber,
            ExpMonth = dto.Card.ExpMonth,
            ExpYear = dto.Card.ExpYear,
            CVV = dto.Card.CVV
        };

        var pm = await _stripeService.CreatePaymentMethodAsync(card);
        return pm.Id;
    }

    public async Task<string> CreateCheckoutSessionAsync(CheckoutSessionRequestDto request)
    {
        // Validación de stock
        foreach (var item in request.LineItems)
        {
            var metadata = item.PriceData.ProductData.Metadata;
            if (!metadata.TryGetValue("productId", out var productIdStr) ||
                !int.TryParse(productIdStr, out var productId))
                throw new ArgumentException("Falta el ProductId en los datos del producto.");

            var product = await _repository.GetProductByIdAsync(productId);
            if (product == null)
                throw new ApplicationException($"Producto con ID {productId} no encontrado.");

            var cantidadSolicitada = (int)item.Quantity;
            if (product.Stock < cantidadSolicitada)
                throw new ApplicationException(
                    $"Stock insuficiente para el producto '{product.Name}'. Solo quedan {product.Stock} unidades.");
        }


        var user = await _currentUserService.GetUser();
        var userId = user!.Id;
        var address = await _repository.GetLatestShippingAddressByUserIdAsync(userId);

        var shippingCost = address?.ShippingCost ?? 0;
        var shippingLabel = shippingCost == 19.99m
            ? "Envío express (1-2 días)"
            : "Envío estándar (3-5 días)";

        // Crear sesión de Stripe
        var session = await _stripeService.CreateCheckoutSessionAsync(
            request,
            shippingCost,
            shippingLabel,
            "http://localhost:5173/success?session_id={CHECKOUT_SESSION_ID}",
            "http://localhost:5173/cancel");

        _logger.LogInformation("Sesión de Stripe creada correctamente. URL: {Url}", session.Url);
        return session.Url;
    }

    public async Task<bool> ProcessWebhookAsync(string json, string signature)
    {
        var secret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
        var stripeEvent = _stripeService.ConstructEvent(json, signature, secret);

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return false;

            _logger.LogInformation("Webhook recibido de Stripe para: {Email}", session.CustomerEmail);

            var user = await _currentUserService.GetUser();
            var userId = user!.Id;


            var cart = await _repository.GetActiveCartByUserIdAsync(userId);
            if (cart is null) return false;

            var shippingAddress = await _repository.GetLatestShippingAddressByUserIdAsync(userId);

            var order = new Order
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                Total = (decimal)(session.AmountTotal / 100.0)!,
                Status = OrderStatus.Paid,
                ShippingAddressId = shippingAddress?.Id,
                ShippingAddress = shippingAddress!
            };

            foreach (var item in cart.CartShopDetails)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            order.Transactions.Add(new Transaction
            {
                PaymentDate = DateTime.UtcNow,
                Amount = order.Total,
                PaymentMethod = PaymentMethod.Card,
                PaymentStatus = PaymentStatus.Success
            });

            cart.Status = CartShopStatus.Completed;
            await _repository.SaveOrderAsync(order);
            await _repository.UpdateCartStatusAsync(cart);

            _logger.LogInformation("Orden con dirección guardada correctamente con ID {OrderId}", order.Id);
        }

        return true;
    }

    public async Task<object> GetSessionDetailsAsync(string sessionId)
    {
        return await _stripeService.GetSessionAsync(sessionId);
    }

    public async Task SaveCartAsync(SaveCartDto dto)
    {
        var cart = new CartShop
        {
            UserId = dto.UserId,
            CreationDate = DateTime.UtcNow,
            Status = CartShopStatus.Active
        };

        foreach (var item in dto.Items)
        {
            var exists = await _repository.ProductExistsAsync(item.ProductId);
            if (!exists)
                throw new ApplicationException($"Producto con ID {item.ProductId} no existe.");

            cart.CartShopDetails.Add(new CartShopDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            });
        }

        await _repository.SaveCartAsync(cart);
    }
}