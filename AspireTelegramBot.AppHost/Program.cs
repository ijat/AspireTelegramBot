var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServerContainer("sql", "hsjkHEJRK34@@!!", 58080).AddDatabase("TempDb");

var messaging = builder.AddRabbitMQContainer("messaging");

builder.AddContainer("botapi", "aiogram/telegram-bot-api")
    .WithEnvironment("TELEGRAM_API_ID", Environment.GetEnvironmentVariable("TELEGRAM_API_ID"))
    .WithEnvironment("TELEGRAM_API_HASH", Environment.GetEnvironmentVariable("TELEGRAM_API_HASH"))
    .WithEnvironment("TELEGRAM_STAT", "1")
    .WithEnvironment("TELEGRAM_LOCAL", "1")
    .WithVolumeMount("telegram-bot-api-data", "/var/lib/telegram-bot-api", VolumeMountType.Named)
    .WithServiceBinding(8081, 8081, "http", "api")
    .WithServiceBinding(8082, 8082, "http", "stat");

builder.AddProject<Projects.CommandsService>("commandsservice")
    .WithReference(sql)
    .WithReference(messaging);

builder.AddProject<Projects.GatewayService>("gatewayservice")
    .WithReference(messaging)
    .WithEnvironment("GATEWAY_HOST", Environment.GetEnvironmentVariable("GATEWAY_HOST"))
    .WithEnvironment("TELEGRAM_API_HOST", Environment.GetEnvironmentVariable("TELEGRAM_API_HOST"))
    .WithEnvironment("TELEGRAM_BOT_TOKEN", Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"));

builder.AddProject<Projects.MessagesService>("messagesservice")
    .WithReference(sql)
    .WithReference(messaging);

builder.AddProject<Projects.SenderService>("senderservice")
    .WithReference(messaging)
    .WithEnvironment("TELEGRAM_API_HOST", Environment.GetEnvironmentVariable("TELEGRAM_API_HOST"))
    .WithEnvironment("TELEGRAM_BOT_TOKEN", Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"));

builder.Build().Run();
