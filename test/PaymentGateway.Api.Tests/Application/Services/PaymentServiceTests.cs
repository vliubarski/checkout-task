using Moq;
using Microsoft.Extensions.Logging;
using PaymentGateway.Api.Application.Services;
using PaymentGateway.Api.Application.Validators;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Infrastructure.Gateways;
using PaymentGateway.Api.Infrastructure.Repositories;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Application.Dto;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentsRepository> _repoMock = new();
    private readonly Mock<IPaymentValidator> _validatorMock = new();
    private readonly Mock<IBankService> _bankServiceMock = new();
    private readonly Mock<ILogger<PaymentService>> _loggerMock = new();

    private PaymentService CreateService() =>
        new(_repoMock.Object, _validatorMock.Object, _bankServiceMock.Object, _loggerMock.Object);

    [Fact]
    public async Task ProcessPayment_ShouldReturnRejected_WhenValidationFails()
    {
        // Arrange
        var request = CreateValidRequest();
        _validatorMock.Setup(v => v.Validate(request))
                      .Returns(new PaymentValidatorResult (false, "Invalid data"));
        var service = CreateService();

        // Act
        var result = await service.ProcessPayment(request);

        // Assert
        Assert.Equal(PaymentStatus.Rejected.ToString(), result.PaymentStatus);
        Assert.Equal("Invalid data", result.ErrorMessage);
        _loggerMock.Verify(l => l.Log(
            It.Is<LogLevel>(lvl => lvl == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Payment Rejected")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnDeclined_WhenBankDoesNotAuthorize()
    {
        // Arrange
        var request = CreateValidRequest();
        _validatorMock.Setup(v => v.Validate(request))
                      .Returns(new PaymentValidatorResult(true, null));
        _bankServiceMock.Setup(b => b.MakePayment(request, new CancellationToken()))
                        .ReturnsAsync(new GatewayPaymentResponse
                        {
                            authorized = false,
                            authorization_code = Guid.NewGuid()
                        });

        var service = CreateService();

        // Act
        var result = await service.ProcessPayment(request);

        // Assert
        Assert.Equal(PaymentStatus.Declined.ToString(), result.PaymentStatus);
        Assert.Equal("Payment is Declined by bank", result.ErrorMessage);
        _repoMock.Verify(r => r.Add(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnAuthorized_WhenBankAuthorizes()
    {
        // Arrange
        var request = CreateValidRequest();
        var authCode = Guid.NewGuid();

        _validatorMock.Setup(v => v.Validate(request))
                      .Returns(new PaymentValidatorResult(true, null));

        _bankServiceMock.Setup(b => b.MakePayment(request, new CancellationToken()))
                        .ReturnsAsync(new GatewayPaymentResponse
                        {
                            authorized = true,
                            authorization_code = authCode
                        });

        var service = CreateService();

        // Act
        var result = await service.ProcessPayment(request);
        var requestLastFour = int.Parse(request.CardNumber[^4..]);
        // Assert
        Assert.Null(result.ErrorMessage);
        Assert.NotNull(result.Payment);

        Assert.Equal(PaymentStatus.Authorized.ToString(), result.PaymentStatus);
        Assert.Equal(request.Amount, result.Payment!.Amount);
        Assert.Equal(requestLastFour, result.Payment.CardNumberLastFour);
        Assert.Equal(request.Currency, result.Payment.Currency);
        Assert.Equal(request.ExpiryMonth, result.Payment.ExpiryMonth);
        Assert.Equal(request.ExpiryYear, result.Payment.ExpiryYear);

        _repoMock.Verify(r => r.Add(It.Is<Payment>(p => p.CardNumberLastFour == requestLastFour && p.Status == PaymentStatus.Authorized)), Times.Once);

        _loggerMock.Verify(l => l.Log(
            It.Is<LogLevel>(lvl => lvl == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("processed with status Authorized")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullResponse_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.Get(id)).ReturnsAsync((Payment?)null);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        Assert.IsType<GetPaymentNullResponse>(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedPayment_WhenExists()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            Amount = 100,
            Currency = "USD",
            CardNumberLastFour = 1234,
            ExpiryMonth = 12,
            ExpiryYear = 2030
        };
        _repoMock.Setup(r => r.Get(payment.Id)).ReturnsAsync(payment);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(payment.Id);

        // Assert
        Assert.IsType<GetPaymentResponse>(result);
        Assert.Equal(payment.Id, result.Id);
        Assert.Equal(payment.Status, result.Status);
        Assert.Equal(payment.Amount, result.Amount);
    }

    private PostPaymentRequest CreateValidRequest() => new()
    {
        CardNumber = "4111111111111111",
        ExpiryMonth = 12,
        ExpiryYear = DateTime.UtcNow.Year + 1,
        Currency = "USD",
        Amount = 100,
        Cvv = "123"
    };
}
