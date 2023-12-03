using CommonLib.Views;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SenderService
{
    public class SenderHostedService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly string ApiHost = Environment.GetEnvironmentVariable("TELEGRAM_API_HOST") ?? throw new NullReferenceException(nameof(ApiHost));
        private readonly string BotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? throw new NullReferenceException(nameof(BotToken));

        public SenderHostedService(IConnection connection, IHttpClientFactory httpClientFactory) 
        {
            _connection = connection;
            _httpClientFactory = httpClientFactory;

            var channel = _connection.CreateModel();
            channel.QueueDeclare(CommonLib.Constants.RabbitMqConstants.SenderQueueName, false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (ch, ea) =>
            {
                var rawBytes = ea.Body.ToArray();

                if (rawBytes.Length == 0)
                    return;

                var jsonString = Encoding.UTF8.GetString(rawBytes, 0, rawBytes.Length);

                if (string.IsNullOrEmpty(jsonString))
                    return;

                var senderView = JsonSerializer.Deserialize<SenderView>(jsonString);

                if (senderView is null)
                    return;

                var httpClient = _httpClientFactory.CreateClient();
                var postBody = new
                {
                    chat_id = senderView.ChatId,
                    text = senderView.Text
                };

                var postRequest = await httpClient.PostAsJsonAsync($"http://{ApiHost}:8081/bot{BotToken}/sendMessage", postBody);
            };

            string consumerTag = channel.BasicConsume(CommonLib.Constants.RabbitMqConstants.SenderQueueName, true, consumer);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}
