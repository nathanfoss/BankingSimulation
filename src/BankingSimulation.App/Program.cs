using BankingSimulation.Application.Commands;
using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.Events;
using BankingSimulation.Infrastructure.AccountHolders;
using BankingSimulation.Infrastructure.AccountLogs;
using BankingSimulation.Infrastructure.Accounts;
using BankingSimulation.Infrastructure.Events;
using BankingSimulation.Infrastructure.Simulator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(_ => _.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true))
    .ConfigureServices(services =>
    {
        services.AddLogging()
            .AddSingleton<IAccountService, AccountService>()
            .AddSingleton<IAccountHolderService, AccountHolderService>()
            .AddSingleton<IAccountLogService, AccountLogService>()
            .AddSingleton<IAccountEventService, AccountEventService>()
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetAccountQuery).Assembly);
            })
            .AddHostedService<AccountEventBackgroundService>()
            .AddHostedService<AccountSimulatorBackgroundService>();
    });

using var host = hostBuilder.Build();
await host.RunAsync();