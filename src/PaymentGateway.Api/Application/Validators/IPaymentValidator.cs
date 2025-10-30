using PaymentGateway.Api.Application.Dto;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Application.Validators
{
    public interface IPaymentValidator
    {
        PaymentValidatorResult Validate(PostPaymentRequest request);
    }
}