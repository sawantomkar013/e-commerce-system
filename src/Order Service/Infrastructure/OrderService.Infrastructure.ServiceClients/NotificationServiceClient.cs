using Microsoft.Extensions.Logging;
using OrderService.Domain.DataAccess.Entities;
using OrderService.Infrastructure.Helpers;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.ServiceClients;

public class NotificationServiceClient : INotificationClient
{
    private readonly HttpClient _http;
    private readonly ILogger<NotificationServiceClient> _logger;

    public NotificationServiceClient(HttpClient http, ILogger<NotificationServiceClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<NotificationResult> SendAsync(OrderStatus orderStatus, object payload, CancellationToken cancellationToken)
    {
        var retryPolicy = CreatePollyMechanism(orderStatus);

        try
        {
            HttpResponseMessage response = await retryPolicy.ExecuteAsync(async () =>
            {
                var resp = await _http.PostAsJsonAsync("/api/v1/notifications", payload, cancellationToken);
                resp.EnsureSuccessStatusCode();
                return resp;
            });

            return new NotificationResult
            {
                Success = true,
                Message = "Notification delivered successfully",
                StatusCode = (int)response.StatusCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Notification delivery failed after retries.");
            return new NotificationResult
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 422 // Use 422 or appropriate code
            };
        }
    }

    private AsyncRetryPolicy<HttpResponseMessage> CreatePollyMechanism(OrderStatus orderStatus)
    {
        var initialDelay = OrderProcessingHelper.GetNotificationDelay(orderStatus);
        var retryAttempts = OrderProcessingHelper.GetRetryAttempts(orderStatus);

        // Define exponential backoff retry policy
        return Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: retryAttempts,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1)),
                onRetryAsync: async (outcome, timespan, attempt, context) =>
                {
                    if (outcome.Exception != null)
                        _logger.LogWarning(outcome.Exception, "Notify failed (attempt {Attempt}/{Max}). Retrying in {Delay}...", attempt, retryAttempts, timespan);
                    else
                        _logger.LogWarning("Notify failed with status {StatusCode} (attempt {Attempt}/{Max}). Retrying in {Delay}...",
                            outcome.Result.StatusCode, attempt, retryAttempts, timespan);
                    await Task.CompletedTask;
                });
    }
}
