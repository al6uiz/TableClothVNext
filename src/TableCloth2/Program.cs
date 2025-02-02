using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TableCloth2;
using TableCloth2.Shared.Models;
using TableCloth2.Shared.Services;
using TableCloth2.Spork;
using TableCloth2.TableCloth;

var arguments =
    //new string[] { "--mode=Spork" }
    args
    ;
var preServices = TableCloth2Configurator.CreateOuterApplication(arguments);
var config = preServices.GetRequiredService<Configurations>();
var utility = preServices.GetRequiredService<ProcessUtility>();

if (utility.HasPrivilegeRequested && !utility.HasAdministratorRole())
{
    utility.Restart(true);
    return;
}

var builder = Host.CreateApplicationBuilder(args);
builder.ConfigureTableCloth2();
builder.Logging.AddConsole();

var isSporkRequired =
    config.Mode == ApplicationMode.Spork ||
    string.Equals("WDAGUtilityAccount", Environment.UserName, StringComparison.OrdinalIgnoreCase);

if (isSporkRequired)
    builder.ConfigureSpork();
else
    builder.ConfigureTableCloth();

var app = builder.Build();

app.Run();
