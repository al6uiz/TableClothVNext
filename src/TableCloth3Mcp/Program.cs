using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3Mcp;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<LauncherMcpWorker>();

var app = builder.Build();
app.Run();
