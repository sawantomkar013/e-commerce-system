using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.ServiceClients.UnitTests;

public class NotificationServiceClientTests
{
    private NotificationServiceClient CreateClient(HttpMessageHandler handler, out Mock<ILogger<NotificationServiceClient>> loggerMock)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
        loggerMock = new Mock<ILogger<NotificationServiceClient>>();
        return new NotificationServiceClient(httpClient, loggerMock.Object);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccess_WhenHttpSucceeds()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var client = CreateClient(handlerMock.Object, out _);

        // Act
        var result = await client.SendAsync(OrderStatus.Pending, new { data = "test" }, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Notification delivered successfully", result.Message);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnFailure_WhenHttpThrowsException()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network failure"));

        var client = CreateClient(handlerMock.Object, out var loggerMock);

        // Act
        var result = await client.SendAsync(OrderStatus.Pending, new { data = "test" }, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Network failure", result.Message);
        Assert.Equal(422, result.StatusCode);

        // Logger should have been called
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Notification delivery failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SendAsync_ShouldRetry_OnHttpStatusFailure()
    {
        // Arrange
        int callCount = 0;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(() =>
            {
                callCount++;
                // Fail first 2 times
                return callCount < 3
                    ? new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    : new HttpResponseMessage(HttpStatusCode.OK);
            });

        var client = CreateClient(handlerMock.Object, out var loggerMock);

        // Act
        var result = await client.SendAsync(OrderStatus.Pending, new { data = "test" }, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(3, callCount); // retried twice before success

        // Logger should have been called for retries
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Notify failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Exactly(2)
        );
    }
}
