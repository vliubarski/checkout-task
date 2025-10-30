using PaymentGateway.Api.Application.Dto;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Models.Requests;
using static PaymentGateway.Api.Shared.ValidationMessagesnamespace;

namespace PaymentGateway.Api.Application.Validators;

public class PaymentValidator : IPaymentValidator
{
    public PaymentValidatorResult Validate(PostPaymentRequest request)
    {
        if (request == null)
            return new(false, ValidationMessages.RequestIsNull);

        if (!IsValidCardNumber(request.CardNumber))
            return new(false, ValidationMessages.InvalidCardNumber);

        if (!IsValidExpiry(request.ExpiryMonth, request.ExpiryYear))
            return new(false, ValidationMessages.InvalidExpiryDate);

        if (!IsSupportedCurrency(request.Currency))
            return new(false, $"{ValidationMessages.UnsupportedCurrency} '{request.Currency}'");

        if (request.Amount <= 0)
            return new(false, ValidationMessages.InvalidAmount);

        if (!IsValidCvv(request.Cvv))
            return new(false, ValidationMessages.InvalidCvv);

        return new(true, null);
    }

    private static bool IsValidCardNumber(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        cardNumber = cardNumber.Replace(" ", "");
        return cardNumber.All(char.IsDigit) && cardNumber.Length is >= 14 and <= 19;
    }

    private static bool IsValidExpiry(int month, int year)
    {
        if (month is < 1 or > 12)
            return false;

        var expiry = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        return expiry > DateTime.UtcNow;
    }

    private static bool IsSupportedCurrency(string? currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return false;

        return Enum.TryParse(typeof(Currency), currency, true, out _);
    }

    private static bool IsValidCvv(string? cvv)
        => !string.IsNullOrWhiteSpace(cvv) && cvv.All(char.IsDigit) && (cvv.Length is 3 or 4);
}
