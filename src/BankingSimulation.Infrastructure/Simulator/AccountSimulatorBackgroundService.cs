using BankingSimulation.Application.Commands;
using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.AccountTypes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BankingSimulation.Infrastructure.Simulator
{
    public class AccountSimulatorBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        private TimeSpan period = TimeSpan.FromSeconds(5);

        public AccountSimulatorBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var mediatr = scope.ServiceProvider.GetService<IMediator>();

            var accountHolder1 = new AccountHolder
            {
                Id = Guid.NewGuid(),
                FullName = "Test Person 1",
                PublicIdentifier = Guid.NewGuid()
            };

            var account1 = new Account
            {
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeEnum.Savings,
                AccountHolder = accountHolder1,
                Balance = 0.0m
            };
            var account2 = new Account
            {
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeEnum.Checking,
                AccountHolder = accountHolder1,
                Balance = 0.0m,
                LinkedAccountId = account1.Id,
                LinkedAccount = account1
            };

            var account1Result = await mediatr.Send(new AddAccountCommand
            {
                Account = account1
            });
            var account2Result = await mediatr.Send(new AddAccountCommand
            {
                Account = account2
            });
            var getAccount1Result = await mediatr.Send(new GetAccountQuery
            {
                AccountId = account1.Id,
            });
            Console.WriteLine(getAccount1Result.Response.ToString());

            var deposit1Response = await mediatr.Send(new DepositMoneyCommand
            {
                AccountId = account1.Id,
                Amount = 100
            });

            Console.WriteLine(deposit1Response.Response.ToString());

            var deposit2Response = await mediatr.Send(new DepositMoneyCommand
            {
                AccountId = account2.Id,
                Amount = 1000
            });

            Console.WriteLine(deposit2Response.Response.ToString());

            var withdraw1Response = await mediatr.Send(new WithdrawMoneyCommand
            {
                AccountId = account1.Id,
                Amount = 10
            });

            Console.WriteLine(withdraw1Response.Response.ToString());

            await mediatr.Send(new TransferMoneyCommand
            {
                FromAccountId = account1.Id,
                ToAccountId = account2.Id,
                Amount = 10
            });

            var account1Response = await mediatr.Send(new GetAccountQuery { AccountId = account1.Id });
            Console.WriteLine(account1Response.Response.ToString());

            var account2Response = await mediatr.Send(new GetAccountQuery { AccountId = account2.Id });
            Console.WriteLine(account2Response.Response.ToString());

            Console.WriteLine("Waiting for events");
            using var timer = new PeriodicTimer(period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync())
            {
                var account1LogsResponse = await mediatr.Send(new GetAccountLogsQuery { AccountId = account1.Id });
                var logs = string.Empty;
                foreach (var log in account1LogsResponse.Response)
                {
                    logs += log.ToString();
                    logs += "\r\n";
                }
                Console.WriteLine(logs);

                logs = string.Empty;
                var account2LogsResponse = await mediatr.Send(new GetAccountLogsQuery { AccountId = account2.Id });
                foreach (var log in account2LogsResponse.Response)
                {
                    logs += log.ToString();
                    logs += "\r\n";
                }
                Console.WriteLine(logs);
            }
        }
    }
}
