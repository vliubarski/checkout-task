using System.Text.Json;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Infrastructure.Gateways;

public class BankService : IBankService
{
    readonly string _url = "http://localhost:8080/payments";

    private record GatewayPaymentRequest(
           string card_number,
           string expiry_date,
           string currency,
           int amount,
           string cvv
       );
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly HttpClient _httpClient;

    public BankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<GatewayPaymentResponse> MakePayment(PostPaymentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var gatewayPaymentRequest = new GatewayPaymentRequest(
            card_number: request.CardNumber,
            expiry_date: $"{request.ExpiryMonth}/{request.ExpiryYear}",
            currency: request.Currency,
            amount: request.Amount,
            cvv: request.Cvv
        );

        var response = await _httpClient.PostAsJsonAsync(_url, gatewayPaymentRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var paymentResponse = await response.Content.ReadFromJsonAsync<GatewayPaymentResponse>(_jsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Bank returned an empty response.");

        return paymentResponse;
    }
}
