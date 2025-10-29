using PaymentGateway.Api.Converters;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Responses;

public class GatewayPaymentResponse
{
    public bool authorized { get; set; }

    [JsonConverter(typeof(NullableGuidConverter))]
    public Guid? authorization_code { get; set; }
}