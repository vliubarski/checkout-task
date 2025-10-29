using System.Text.Json;

using PaymentGateway.Api.Converters;

public class NullableGuidConverterTests
{
    private readonly JsonSerializerOptions _options;

    public NullableGuidConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new NullableGuidConverter());
    }

    [Fact]
    public void Read_ShouldReturnNull_WhenJsonValueIsNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<Guid?>(json, _options);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Read_ShouldReturnEmptyGuid_WhenStringIsEmpty()
    {
        // Arrange
        var json = "\"\"";

        // Act
        var result = JsonSerializer.Deserialize<Guid?>(json, _options);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void Read_ShouldReturnEmptyGuid_WhenStringIsWhitespace()
    {
        // Arrange
        var json = "\"   \"";

        // Act
        var result = JsonSerializer.Deserialize<Guid?>(json, _options);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void Read_ShouldReturnGuid_WhenStringIsValidGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var json = $"\"{guid}\"";

        // Act
        var result = JsonSerializer.Deserialize<Guid?>(json, _options);

        // Assert
        Assert.Equal(guid, result);
    }

    [Fact]
    public void Read_ShouldThrowJsonException_WhenStringIsInvalidGuid()
    {
        // Arrange
        var json = "\"not-a-guid\"";

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Guid?>(json, _options));

        Assert.Contains("Invalid GUID value", ex.Message);
    }

    [Fact]
    public void Read_ShouldThrowJsonException_WhenTokenIsNotStringOrNull()
    {
        // Arrange
        var json = "123"; // Not a string or null

        // Act & Assert
        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Guid?>(json, _options));

        Assert.Contains("Unexpected token", ex.Message);
    }

    [Fact]
    public void Write_ShouldWriteNull_WhenValueIsNull()
    {
        // Arrange
        Guid? value = null;

        // Act
        var json = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal("null", json);
    }

    [Fact]
    public void Write_ShouldWriteString_WhenValueIsGuid()
    {
        // Arrange
        Guid? value = Guid.NewGuid();

        // Act
        var json = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal($"\"{value}\"", json);
    }
}
