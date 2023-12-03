using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace GatewayService
{
    public class GatewayHostedService : BackgroundService
    {
        private readonly IServer _server;

        public GatewayHostedService(IServer server) 
        {
            _server = server;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(3000);

            var httpClient = new HttpClient();
            var apiHost = Environment.GetEnvironmentVariable("TELEGRAM_API_HOST");
            var gwHost = Environment.GetEnvironmentVariable("GATEWAY_HOST");
            var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
            var feature = _server.Features.Get<IServerAddressesFeature>();
            var gwPort = feature?.Addresses.First().Split(":")[2];

            if (string.IsNullOrEmpty(botToken))
                throw new NullReferenceException(nameof(botToken));

            if (string.IsNullOrEmpty(gwPort))
                throw new NullReferenceException(nameof(gwPort));

            var httpResponseMessage = await httpClient.GetAsync($"http://{apiHost}:8081/bot{botToken}/setWebhook?url=http://{gwHost}:{gwPort}/update&max_connections=100000");
            httpResponseMessage.EnsureSuccessStatusCode();
        }
    }
}
