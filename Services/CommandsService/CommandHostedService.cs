using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using CommonLib.Views;
using System.Text.Json;
using System.Text;
using CommonLib.Repository;
using Microsoft.EntityFrameworkCore;

namespace CommandsService;

public class CommandHostedService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment _env;

    public CommandHostedService(IConnection connection, IServiceScopeFactory scopeFactory, IWebHostEnvironment env)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _env = env;

        var consumerChannel = _connection.CreateModel();
        var producerChannel = _connection.CreateModel();
        consumerChannel.QueueDeclare(CommonLib.Constants.RabbitMqConstants.CommandsQueueName, false, false, false, null);

        var consumer = new EventingBasicConsumer(consumerChannel);
        consumer.Received += async (ch, ea) =>
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
                Text = $"Hello {updateView.message.from.first_name}!\nYour text: {updateView.message.text}\nSent from `Command Service` at {DateTime.UtcNow.ToString("R")}"
            };

            using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CommonDbContext>();
            if (!await dbContext.Users.AnyAsync(x => x.UserId == updateView.message.chat.id))
            {
                dbContext.Users.Add(new CommonLib.Repository.Models.User()
                {
                    UserId = updateView.message.chat.id,
                    Username = updateView.message.from.username
                });
                await dbContext.SaveChangesAsync();
            }

            var senderJson = JsonSerializer.Serialize(senderView);
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(senderJson);
            producerChannel.BasicPublish(string.Empty, CommonLib.Constants.RabbitMqConstants.SenderQueueName, null, messageBodyBytes);
        };

        string consumerTag = consumerChannel.BasicConsume(CommonLib.Constants.RabbitMqConstants.CommandsQueueName, true, consumer);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_env.IsDevelopment())
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<CommonDbContext>();
                await context!.Database.MigrateAsync(stoppingToken);
            }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
    }
}
