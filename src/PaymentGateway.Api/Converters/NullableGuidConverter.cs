using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Converters;

public class NullableGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                return Guid.Empty;

            if (Guid.TryParse(s, out var guid))
                return guid;

            throw new JsonException($"Invalid GUID value");
        }

        throw new JsonException($"Unexpected token parsing GUID. TokenType: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString());
        else
            writer.WriteNullValue();
    }
}
