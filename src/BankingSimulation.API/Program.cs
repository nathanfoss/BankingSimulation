using BankingSimulation.Application.Queries;
using BankingSimulation.Domain;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.AccountTypes;
using BankingSimulation.Domain.Events;
using BankingSimulation.Infrastructure.AccountHolders;
using BankingSimulation.Infrastructure.AccountLogs;
using BankingSimulation.Infrastructure.Accounts;
using BankingSimulation.Infrastructure.AccountTypes;
using BankingSimulation.Infrastructure.Events;
using BankingSimulation.Infrastructure.Simulator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddLogging()
    .AddDbContext<BankDbContext>()
    .AddTransient<IAccountService, AccountService>()
    .AddTransient<IAccountHolderService, AccountHolderService>()
    .AddTransient<IAccountLogService, AccountLogService>()
    .AddTransient<IAccountEventService, AccountEventService>()
    .AddTransient<IAccountTypeService, AccountTypeService>()
    .AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(GetAccountQuery).Assembly);
    })
    .AddHostedService<AccountEventBackgroundService>()
    .AddHostedService<EnumSeedBackgroundService>()
    .AddSwaggerGen()
    .AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseCors(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
});

app.Run();
