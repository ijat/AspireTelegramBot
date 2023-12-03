using SenderService;
using System.Reflection;

var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.0";
var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<SenderHostedService>();
builder.Services.AddHttpClient<SenderHostedService>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.MapGet("/version", () => version);

app.Run();
