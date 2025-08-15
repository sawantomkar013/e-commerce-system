using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Moq;
using OrderService.Domain.DataAccess.Entities;

namespace OrderService.Infrastructure.BackgroundJobs.UnitTests;

public class KafkaProducerTests
{
    private readonly KafkaSettings _settings = new()
    {
        BaseTopic = "orders",
        BootstrapServers = "localhost:9092"
    };

    [Fact]
    public async Task ProduceAsync_ShouldReturnSuccess_WhenProducerSucceeds()
    {
        // Arrange
        var mockProducer = new Mock<IProducer<string, string>>();
        var tpOffset = new TopicPartitionOffset("orders.pending", 0, 42);

        mockProducer
            .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResult<string, string> { TopicPartitionOffset = tpOffset });

        var producer = new KafkaProducer(Options.Create(_settings), mockProducer.Object);

        // Act
        var result = await producer.ProduceAsync(OrderStatus.Pending, "key1", "value1");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(tpOffset, result.Offset);
    }

    [Fact]
    public async Task ProduceAsync_ShouldReturnFailure_WhenProduceThrowsProduceException()
    {
        // Arrange
        var mockProducer = new Mock<IProducer<string, string>>();
        mockProducer
            .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProduceException<string, string>(new Error(ErrorCode.Local_MsgTimedOut), null));

        var producer = new KafkaProducer(Options.Create(_settings), mockProducer.Object);

        // Act
        var result = await producer.ProduceAsync(OrderStatus.Pending, "key1", "value1");

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorReason);
    }

    [Fact]
    public async Task ProduceAsync_ShouldReturnFailure_WhenProduceThrowsGenericException()
    {
        // Arrange
        var mockProducer = new Mock<IProducer<string, string>>();
        mockProducer
            .Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        var producer = new KafkaProducer(Options.Create(_settings), mockProducer.Object);

        // Act
        var result = await producer.ProduceAsync(OrderStatus.Pending, "key1", "value1");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Something went wrong", result.ErrorReason);
    }

    [Fact]
    public void Dispose_ShouldCallProducerDispose()
    {
        // Arrange
        var mockProducer = new Mock<IProducer<string, string>>();
        var producer = new KafkaProducer(Options.Create(_settings), mockProducer.Object);

        // Act
        producer.Dispose();

        // Assert
        mockProducer.Verify(p => p.Dispose(), Times.Once);
    }
}
