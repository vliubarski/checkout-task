using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Application.Validators
{
    public interface IPaymentValidator
    {
        (bool IsValid, string? ErrorMessage) Validate(PostPaymentRequest request);
    }
}