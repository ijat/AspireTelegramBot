using MessagesService;
using System.Reflection;

var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.0";
var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<MessageHostedService>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.MapGet("/version", () => version);
app.Run();
