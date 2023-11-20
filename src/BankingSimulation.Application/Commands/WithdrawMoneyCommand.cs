using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.AccountLogs;

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

        private readonly IAccountLogService accountLogService;

        private readonly ILogger<WithdrawMoneyCommandHandler> logger;

        public WithdrawMoneyCommandHandler(IAccountService accountService, IAccountLogService accountLogService, ILogger<WithdrawMoneyCommandHandler> logger)
        {
            this.accountService = accountService;
            this.accountLogService = accountLogService;
            this.logger = logger;
        }

        public Task<Result<Account>> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    throw new ArgumentException("Invalid deposit amount");
                }

                var account = accountService.Get(request.AccountId);
                if (account is null)
                {
                    throw new Exception($"Account {request.AccountId} does not exist");
                }

                if (account.Balance < request.Amount)
                {
                    throw new Exception($"Account {request.AccountId} has insufficient funds to complete this withdrawal");
                }

                account.Balance -= request.Amount;
                var result = accountService.Update(account);
                AddAccountLog(result, request.Amount);
                return Task.FromResult(Result<Account>.Success(result));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred withdrawing money from account {Account}", request.AccountId);
                return Task.FromResult(Result<Account>.Failure(ex));
            }
        }

        private void AddAccountLog(Account account, decimal amount)
        {
            accountLogService.Add(new AccountLog
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                CreatedDate = DateTime.UtcNow,
                EventType = AccountEventType.Withdraw,
                Metadata = new Dictionary<string, string>()
                {
                    { "Amount", $"{amount}" },
                    { "Balance", $"{account.Balance}" }
                }
            });
        }
    }
}
