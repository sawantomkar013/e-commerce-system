using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.ServiceClients
{
    public class NotificationServiceClient : INotificationClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<NotificationServiceClient> _logger;

        public NotificationServiceClient(HttpClient http, ILogger<NotificationServiceClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task SendAsync(object payload, int retryAttempts, CancellationToken ct)
        {
            var delay = TimeSpan.FromMilliseconds(200);
            for (var attempt = 0; attempt <= retryAttempts; attempt++)
            {
                try
                {
                    var resp = await _http.PostAsJsonAsync("/api/v1/notifications", payload, ct);
                    resp.EnsureSuccessStatusCode();
                    _logger.LogInformation("Notification delivered after {Attempt} attempt(s)", attempt + 1);
                    return;
                }
                catch (Exception ex) when (attempt < retryAttempts)
                {
                    _logger.LogWarning(ex, "Notify failed (attempt {Attempt}/{Max}). Retrying in {Delay}...", attempt + 1, retryAttempts + 1, delay);
                    await Task.Delay(delay, ct);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
                }
            }
        }
    }
}
