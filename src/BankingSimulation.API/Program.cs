using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.Events;
using BankingSimulation.Infrastructure.AccountHolders;
using BankingSimulation.Infrastructure.AccountLogs;
using BankingSimulation.Infrastructure.Accounts;
using BankingSimulation.Infrastructure.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddLogging()
    .AddSingleton<IAccountService, AccountService>()
    .AddSingleton<IAccountHolderService, AccountHolderService>()
    .AddSingleton<IAccountLogService, AccountLogService>()
    .AddSingleton<IAccountEventService, AccountEventService>()
    .AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(GetAccountQuery).Assembly);
    })
    .AddHostedService<AccountEventBackgroundService>()
    .AddSwaggerGen()
    .AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
