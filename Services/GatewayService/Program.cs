using CommonLib.Views;
using GatewayService;
using RabbitMQ.Client;
using System.Reflection;
using System.Text.Json;

var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.0";
var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<GatewayHostedService>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.MapGet("/version", () => version);

app.MapPost("/update", async (IConnection connection, HttpRequest request) =>
{
    var rawBody = await new StreamReader(request.Body).ReadToEndAsync();
    var tgUpdateView = JsonSerializer.Deserialize<TgUpdateView>(rawBody);

    if (tgUpdateView is null)
        return;

    var channel = connection.CreateModel();
    byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(rawBody);
    if (tgUpdateView.message.text.Trim().StartsWith("/", StringComparison.OrdinalIgnoreCase))
        channel.BasicPublish(string.Empty, CommonLib.Constants.RabbitMqConstants.CommandsQueueName, null, messageBodyBytes);
    else
        channel.BasicPublish(string.Empty, CommonLib.Constants.RabbitMqConstants.MessageQueueName, null, messageBodyBytes);
});

app.Run();
