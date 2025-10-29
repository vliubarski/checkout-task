using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Models.Requests;
using static PaymentGateway.Api.Shared.ValidationMessagesnamespace;

namespace PaymentGateway.Api.Application.Validators;

public class PaymentValidator : IPaymentValidator
{
    public (bool IsValid, string? ErrorMessage) Validate(PostPaymentRequest request)
    {
        if (request == null)
            return (false, ValidationMessages.RequestIsNull);

        if (!IsValidCardNumber(request.CardNumber))
            return (false, ValidationMessages.InvalidCardNumber);

        if (!IsValidExpiry(request.ExpiryMonth, request.ExpiryYear))
            return (false, ValidationMessages.InvalidExpiryDate);

        if (!IsSupportedCurrency(request.Currency))
            return (false, $"{ValidationMessages.UnsupportedCurrency} '{request.Currency}'");

        if (request.Amount <= 0)
            return (false, ValidationMessages.InvalidAmount);

        if (!IsValidCvv(request.Cvv))
            return (false, ValidationMessages.InvalidCvv);

        return (true, null);
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


public class PaymentValidatorOld : IPaymentValidator
{
    public (bool IsValid, string? ErrorMessage) Validate(PostPaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CardNumber) ||
            !request.CardNumber.All(char.IsDigit) ||
            request.CardNumber.Length is < 14 or > 19)
            return (false, "Invalid card number");

        if (request.ExpiryMonth < 1 || request.ExpiryMonth > 12)
            return (false, "Invalid expiry month");

        var now = DateTime.UtcNow;
        var expiry = new DateTime(request.ExpiryYear, request.ExpiryMonth, DateTime.DaysInMonth(request.ExpiryYear, request.ExpiryMonth));
        if (expiry <= now)
            return (false, "Card is expired");

        if (!Enum.TryParse(request.Currency.ToUpperInvariant(), true, out Currency res))
            return (false, "Unsupported currency");

        if (request.Amount <= 0)
            return (false, "Invalid amount");

        if (string.IsNullOrWhiteSpace(request.Cvv) ||
            !request.Cvv.All(char.IsDigit) ||
            request.Cvv.Length != 3 && request.Cvv.Length != 4)
            return (false, "Invalid CVV");

        return (true, null);
    }
}
