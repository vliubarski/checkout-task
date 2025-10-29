﻿using PaymentGateway.Api.Domain.Enums;

namespace PaymentGateway.Api.Models.Responses;

public class GetPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class GetPaymentNullResponse : GetPaymentResponse { }