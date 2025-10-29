using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Infrastructure.Gateways
{
    public interface IBankService
    {
        Task<GatewayPaymentResponse> MakePayment(PostPaymentRequest request, CancellationToken cancellationToken = default);
    }
}