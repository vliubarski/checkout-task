using PaymentGateway.Api.Application.Validators;
using PaymentGateway.Api.Models.Requests;

using static PaymentGateway.Api.Shared.ValidationMessagesnamespace;

public class PaymentValidatorTests
{
    private readonly PaymentValidator _validator = new();

    private PostPaymentRequest CreateValidRequest() => new()
    {
        CardNumber = "4111111111111111",
        ExpiryMonth = 12,
        ExpiryYear = DateTime.UtcNow.Year + 1,
        Currency = "USD",
        Amount = 100,
        Cvv = "123"
    };

    [Fact]
    public void Validate_ShouldReturnValid_ForCorrectRequest()
    {
        var request = CreateValidRequest();

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Theory]
    [InlineData("", false, ValidationMessages.InvalidCardNumber)]
    [InlineData("1234567890123", false, ValidationMessages.InvalidCardNumber)]
    [InlineData("12345678901234", true, null)]
    [InlineData("1234567890123456789", true, null)]
    [InlineData("12345678901234567890", false, ValidationMessages.InvalidCardNumber)]
    public void Validate_ShouldFail_WhenCardNumberIsInvalid_Or_Succeeds_When_Valid(string cardNumber, bool result, string ? errorMsg)
    {
        var request = CreateValidRequest();
        request.CardNumber = cardNumber;

        var validationResult = _validator.Validate(request);

        Assert. Equal(result, validationResult.IsValid);
        Assert.Equal(errorMsg, validationResult.ErrorMessage);
    }

    [Theory]
    [InlineData(0, 0, false, ValidationMessages.InvalidExpiryDate)]
    [InlineData(13, 0, false, ValidationMessages.InvalidExpiryDate)]
    [InlineData(12, -1, false, ValidationMessages.InvalidExpiryDate)]
    [InlineData(12, 0, true, null)]
    [InlineData(12, 1, true, null)]
    public void Validate_ShouldFail_WhenCardIsExpired_Or_Succeeds_When_Valid(int month, int deltaYear, bool expectedValid, string expectedErrorMsg)
    {
        var request = CreateValidRequest();
        request.ExpiryYear = DateTime.UtcNow.Year + deltaYear;
        request.ExpiryMonth = month;

        var result = _validator.Validate(request);

        Assert.Equal(expectedValid, result.IsValid);
        Assert.Equal(expectedErrorMsg, result.ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldFail_WhenCurrencyUnsupported()
    {
        var request = CreateValidRequest();
        request.Currency = "JPY";

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(ValidationMessages.UnsupportedCurrency, result.ErrorMessage);
    }

    [Theory]
    [InlineData("12", false)]   
    [InlineData("12a", false)]  
    [InlineData("", false)]     
    [InlineData(null, false)]   
    [InlineData("1234", true)]  
    [InlineData("12345", false)]
    public void Validate_ShouldFail_WhenCvvIsInvalid(string cvv, bool expectedValid)
    {
        var errorMsg = expectedValid ? null : ValidationMessages.InvalidCvv;
        var request = CreateValidRequest();
        request.Cvv = cvv!;

        var result = _validator.Validate(request);

        Assert.Equal(expectedValid, result.IsValid);
        Assert.Equal(errorMsg, result.ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldFail_WhenAmountIsZeroOrNegative()
    {
        var request = CreateValidRequest();
        request.Amount = 0;

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Equal(ValidationMessages.InvalidAmount, result.ErrorMessage);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRequestIsNull()
    {
        var result = _validator.Validate(null!);

        Assert.False(result.IsValid);
        Assert.Equal(ValidationMessages.RequestIsNull, result.ErrorMessage);
    }
}
