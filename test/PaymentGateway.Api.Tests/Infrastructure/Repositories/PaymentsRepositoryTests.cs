using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Infrastructure.Repositories;

public class PaymentsRepositoryTests
{
    [Fact]
    public async Task Add_ShouldStorePayment()
    {
        // Arrange
        var repository = new PaymentsRepository();
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = 100,
            CardNumberLastFour = 1234,
            Currency = "USD",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Status = PaymentStatus.Authorized
        };

        // Act
        repository.Add(payment);
        var result = await repository.Get(payment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payment.Id, result!.Id);
        Assert.Equal(payment.Amount, result.Amount);
    }

    [Fact]
    public async Task Get_ShouldReturnNull_WhenPaymentNotFound()
    {
        // Arrange
        var repository = new PaymentsRepository();
        var id = Guid.NewGuid();

        // Act
        var result = await repository.Get(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Get_ShouldReturnCorrectPayment_WhenMultiplePaymentsExist()
    {
        // Arrange
        var repository = new PaymentsRepository();

        var payment1 = new Payment { Id = Guid.NewGuid(), Amount = 100 };
        var payment2 = new Payment { Id = Guid.NewGuid(), Amount = 200 };

        repository.Add(payment1);
        repository.Add(payment2);

        // Act
        var result = await repository.Get(payment2.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payment2.Id, result!.Id);
        Assert.Equal(200, result.Amount);
    }
}
