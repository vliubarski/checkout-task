namespace PaymentGateway.Api.Shared;

public class ValidationMessagesnamespace
{
    public static class ValidationMessages
    {
        public const string RequestIsNull = "Request cannot be null";
        public const string InvalidCardNumber = "Invalid card number";
        public const string InvalidExpiryDate = "Invalid or expired card expiry date";
        public const string UnsupportedCurrency = "Unsupported currency";
        public const string InvalidAmount = "Amount must be greater than zero";
        public const string InvalidCvv = "Invalid CVV";
    }
}
