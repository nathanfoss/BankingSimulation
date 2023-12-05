using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Events;
using Newtonsoft.Json;

namespace BankingSimulation.Application.Commands
{
    public class WithdrawMoneyCommand : IRequest<Result<Account>>
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }

    public class WithdrawMoneyCommandHandler : IRequestHandler<WithdrawMoneyCommand, Result<Account>>
    {
        private readonly IAccountService accountService;

        private readonly IAccountEventService accountEventService;

        private readonly ILogger<WithdrawMoneyCommandHandler> logger;

        public WithdrawMoneyCommandHandler(IAccountService accountService, IAccountEventService accountEventService, ILogger<WithdrawMoneyCommandHandler> logger)
        {
            this.accountService = accountService;
            this.accountEventService = accountEventService;
            this.logger = logger;
        }

        public async Task<Result<Account>> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    throw new ArgumentException("Invalid deposit amount");
                }

                var account = await accountService.Get(request.AccountId);
                if (account is null)
                {
                    throw new Exception($"Account {request.AccountId} does not exist");
                }

                if (account.Balance < request.Amount)
                {
                    throw new Exception($"Account {request.AccountId} has insufficient funds to complete this withdrawal");
                }

                account.Balance -= request.Amount;
                var result = await accountService.Update(account);
                await PublishAccountEvent(result, request.Amount);
                return Result<Account>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred withdrawing money from account {Account}", request.AccountId);
                return Result<Account>.Failure(ex);
            }
        }

        private async Task PublishAccountEvent(Account account, decimal amount)
        {
            await accountEventService.Add(new AccountEvent
            {
                Id = Guid.NewGuid(),
                Name = EventTypes.MoneyWithdrawn,
                Payload = JsonConvert.SerializeObject((account, amount))
            });
        }
    }
}
