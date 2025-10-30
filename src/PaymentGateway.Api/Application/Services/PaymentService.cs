using PaymentGateway.Api.Application.Dto;
using PaymentGateway.Api.Application.Validators;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Infrastructure.Gateways;
using PaymentGateway.Api.Infrastructure.Repositories;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentsRepository _repository;
    private readonly IPaymentValidator _validator;
    private readonly IBankService _bankService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentsRepository repository, IPaymentValidator validator, IBankService bankService, ILogger<PaymentService> logger)
    {
        _repository = repository;
        _validator = validator;
        _bankService = bankService;
        _logger = logger;
    }

    public async Task<ProcessPaymentResult> ProcessPayment(PostPaymentRequest request)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning($"Payment Rejected: {validationResult.ErrorMessage}");
            return new ProcessPaymentResult { PaymentStatus = PaymentStatus.Rejected.ToString(), ErrorMessage = validationResult.ErrorMessage };
        }

        var gatewayPaymentResponse = await _bankService.MakePayment(request);

        var status = gatewayPaymentResponse.authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = status,
            CardNumberLastFour = int.Parse(request.CardNumber[^4..]),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency.ToUpperInvariant(),
            Amount = request.Amount
        };

        _repository.Add(payment);
        _logger.LogInformation($"Payment with id: {payment.Id} processed with status {status}");

        return new ProcessPaymentResult
        {
            Payment = payment,
            PaymentStatus = payment.Status.ToString(),
            ErrorMessage = status == PaymentStatus.Declined ? "Payment is Declined by bank" : null
        };
    }

    public async Task<GetPaymentResponse> GetByIdAsync(Guid id)
    {
        var payment = await _repository.Get(id);
        if (payment == null)
            return new GetPaymentNullResponse();

        return new GetPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
}
