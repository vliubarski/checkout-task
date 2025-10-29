using PaymentGateway.Api.Application.Dto;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Application.Services;

public interface IPaymentService
{
    Task<ProcessPaymentResult> ProcessPayment(PostPaymentRequest request);

    Task<GetPaymentResponse> GetByIdAsync(Guid id);
}
