using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;

namespace YaDeliveryB2b.Client.Test;

[TestClass]
[TestCategory("Unit")]
public sealed class YaDeliveryB2bUnitTests
{
    [TestMethod]
    public async Task HistoryTest()
    {
        // Arrange
        using var httpClient = CreateHttpClientMoq(new Response8 { State_history = [ new State_history {} ] });
        var client = new YaDeliveryB2bClient(httpClient);

        // Act
        var response = await client.HistoryAsync("1", default);

        // Assets
        Assert.IsNotNull(response.State_history);
    }

    private static HttpClient CreateHttpClientMoq(object result)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = JsonContent.Create(result),
           });

        var client = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test.com") };
        return client;
    }
}
