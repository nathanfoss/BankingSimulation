using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Infrastructure.AccountHolders;
using BankingSimulation.Infrastructure.Accounts;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

var services = new ServiceCollection()
    .AddLogging()
    .AddSingleton<IAccountService, AccountService>()
    .AddSingleton<IAccountHolderService, AccountHolderService>()
    .AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(GetAccountQuery).Assembly);
    });
var serviceProvider = services.BuildServiceProvider();
var mediatr = serviceProvider.GetRequiredService<IMediator>();