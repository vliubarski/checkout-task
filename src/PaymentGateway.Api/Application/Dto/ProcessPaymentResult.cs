using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Application.Dto;

public class ProcessPaymentResult
{
    public Payment? Payment { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
