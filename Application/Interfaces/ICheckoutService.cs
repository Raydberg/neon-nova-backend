﻿using Application.DTOs.CheckoutDTOs;

namespace Application.Interfaces;

public interface ICheckoutService
{
    Task<int> SavePersonalInfoAsync(PersonalInfoDto dto);
    Task<string> CreatePaymentMethodAsync(PaymentMethodDto dto);
    Task<string> CreateCheckoutSessionAsync(CheckoutSessionRequestDto request);
    Task<bool> ProcessWebhookAsync(string json, string signature);
    Task<object> GetSessionDetailsAsync(string sessionId);
    Task SaveCartAsync(SaveCartDto dto);
}