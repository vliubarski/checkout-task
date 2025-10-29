using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using PaymentGateway.Api.Infrastructure.Gateways;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

public class BankServiceTests
{
    [Fact]
    public async Task MakePayment_ShouldReturnPaymentResponse_WhenBankReturnsSuccess()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            Amount = 100,
            CardNumber = "4111111111111111",
            Currency = "USD",
            Cvv = "123",
            ExpiryMonth = 12,
            ExpiryYear = 2030
        };

        var expectedResponse = new GatewayPaymentResponse
        {
            authorized = true,
            authorization_code = Guid.NewGuid()
        };

        // Serialize expected response
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedResponse)
        };

        // Mock HttpMessageHandler
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // mocked handler for BankService
        var client = new HttpClient(handlerMock.Object);

        var service = new BankService(client);

        // Act
        var result = await service.MakePayment(request);

        // Assert
        Assert.True(result.authorized);
        Assert.Equal(expectedResponse.authorization_code, result.authorization_code);

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri == new System.Uri("http://localhost:8080/payments")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}
