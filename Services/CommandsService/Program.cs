using CommandsService;
using CommonLib.Repository;
using System.Reflection;

var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.0";
var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<CommonDbContext>("TempDb");
builder.Services.AddHostedService<CommandHostedService>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapGet("/version", () => version);
app.Run();
