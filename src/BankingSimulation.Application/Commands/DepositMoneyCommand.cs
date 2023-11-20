using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Domain.Accounts;

namespace BankingSimulation.Application.Commands
{
    public class DepositMoneyCommand : IRequest<Result<Account>>
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }

    public class DepositMoneyCommandHandler : IRequestHandler<DepositMoneyCommand, Result<Account>>
    {
        private readonly IAccountService accountService;

        private readonly ILogger<DepositMoneyCommandHandler> logger;

        public DepositMoneyCommandHandler(IAccountService accountService, ILogger<DepositMoneyCommandHandler> logger)
        {
            this.accountService = accountService;
            this.logger = logger;
        }

        public Task<Result<Account>> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = accountService.Get(request.AccountId);
                if (account is null)
                {
                    throw new Exception($"Account {request.AccountId} does not exist");
                }

                account.Balance += request.Amount;
                return Task.FromResult(Result<Account>.Success(accountService.Update(account)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred depositing money into account {account}", request.AccountId);
                return Task.FromResult(Result<Account>.Failure(ex));
            }
        }
    }
}
