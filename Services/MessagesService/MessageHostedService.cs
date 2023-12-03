using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using CommonLib.Views;
using System.Text.Json;
using System.Text;

namespace MessagesService;

public class MessageHostedService : BackgroundService
{
    private readonly IConnection _connection;

    public MessageHostedService(IConnection connection)
    {
        _connection = connection;

        var consumerChannel = _connection.CreateModel();
        var producerChannel = _connection.CreateModel();

        consumerChannel.QueueDeclare(CommonLib.Constants.RabbitMqConstants.MessageQueueName, false, false, false, null);

        var consumer = new EventingBasicConsumer(consumerChannel);
        consumer.Received += (ch, ea) =>
        {
            var rawBytes = ea.Body.ToArray();

            if (rawBytes.Length == 0)
                return;

            var jsonString = Encoding.UTF8.GetString(rawBytes, 0, rawBytes.Length);

            if (string.IsNullOrEmpty(jsonString))
                return;

            var updateView = JsonSerializer.Deserialize<TgUpdateView>(jsonString);

            if (updateView is null)
                return;

            var senderView = new SenderView()
            {
                ChatId = updateView.message.chat.id,
                Text = $"Hello {updateView.message.from.first_name}!\nYour text: {updateView.message.text}\nSent from `Message Service` at {DateTime.UtcNow.ToString("R")}"
            };

            var senderJson = JsonSerializer.Serialize(senderView);
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(senderJson);
            producerChannel.BasicPublish(string.Empty, CommonLib.Constants.RabbitMqConstants.SenderQueueName, null, messageBodyBytes);
        };

        string consumerTag = consumerChannel.BasicConsume(CommonLib.Constants.RabbitMqConstants.MessageQueueName, true, consumer);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
    }
}
