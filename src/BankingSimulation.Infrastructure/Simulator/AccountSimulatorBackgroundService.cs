using BankingSimulation.Application.Commands;
using BankingSimulation.Application.Models;
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

            var account1 = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Savings,
                AccountHolderName = accountHolder1.FullName,
                AccountHolderPublicIdentifier = accountHolder1.PublicIdentifier
            };

            var account1Result = await mediatr.Send(new AddAccountCommand
            {
                Account = account1
            });

            var account1Id = account1Result.Response;

            var account2 = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Checking,
                AccountHolderName = accountHolder1.FullName,
                AccountHolderPublicIdentifier = accountHolder1.PublicIdentifier,
                LinkedAccountId = account1Id
            };
            var account2Result = await mediatr.Send(new AddAccountCommand
            {
                Account = account2
            });

            var account2Id = account2Result.Response;
            var getAccount1Result = await mediatr.Send(new GetAccountQuery
            {
                AccountId = account1Id,
            });
            Console.WriteLine(getAccount1Result.Response.ToString());

            var deposit1Response = await mediatr.Send(new DepositMoneyCommand
            {
                AccountId = account1Id,
                Amount = 100
            });

            Console.WriteLine(deposit1Response.Response.ToString());

            var deposit2Response = await mediatr.Send(new DepositMoneyCommand
            {
                AccountId = account2Id,
                Amount = 1000
            });

            Console.WriteLine(deposit2Response.Response.ToString());

            var withdraw1Response = await mediatr.Send(new WithdrawMoneyCommand
            {
                AccountId = account1Id,
                Amount = 10
            });

            Console.WriteLine(withdraw1Response.Response.ToString());

            await mediatr.Send(new TransferMoneyCommand
            {
                FromAccountId = account1Id,
                ToAccountId = account2Id,
                Amount = 10
            });

            var account1Response = await mediatr.Send(new GetAccountQuery { AccountId = account1Id });
            Console.WriteLine(account1Response.Response.ToString());

            var account2Response = await mediatr.Send(new GetAccountQuery { AccountId = account2Id });
            Console.WriteLine(account2Response.Response.ToString());

            Console.WriteLine("Waiting for events");
            using var timer = new PeriodicTimer(period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync())
            {
                var account1LogsResponse = await mediatr.Send(new GetAccountLogsQuery { AccountId = account1Id });
                var logs = string.Empty;
                foreach (var log in account1LogsResponse.Response)
                {
                    logs += log.ToString();
                    logs += "\r\n";
                }
                Console.WriteLine(logs);

                logs = string.Empty;
                var account2LogsResponse = await mediatr.Send(new GetAccountLogsQuery { AccountId = account2Id });
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
