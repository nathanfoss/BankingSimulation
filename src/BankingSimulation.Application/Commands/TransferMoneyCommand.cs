using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.Events;
using Newtonsoft.Json;

namespace BankingSimulation.Application.Commands
{
    public class TransferMoneyCommand : IRequest<Result<Account>>
    {
        public Guid FromAccountId { get; set; }

        public Guid ToAccountId { get; set; }

        public decimal Amount { get; set; }
    }

    public class TransferMoneyCommandHandler : IRequestHandler<TransferMoneyCommand, Result<Account>>
    {
        private readonly IAccountService accountService;

        private readonly IAccountEventService accountEventService;

        private readonly ILogger<TransferMoneyCommandHandler> logger;

        public TransferMoneyCommandHandler(IAccountService accountService, IAccountEventService accountEventService, ILogger<TransferMoneyCommandHandler> logger)
        {
            this.accountService = accountService;
            this.accountEventService = accountEventService;
            this.logger = logger;
        }

        public Task<Result<Account>> Handle(TransferMoneyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    throw new ArgumentException("Invalid Transfer amount");
                }


                var fromAccount = ValidateAccount(request.FromAccountId);
                var toAccount = ValidateAccount(request.ToAccountId);

                if (fromAccount.Balance < request.Amount)
                {
                    throw new Exception($"Account {request.FromAccountId} has insufficient funds to complete this transaction");
                }

                fromAccount.Balance -= request.Amount;
                toAccount.Balance += request.Amount;

                var fromResult = accountService.Update(fromAccount);
                var toResult = accountService.Update(toAccount);

                PublishAccountEvent(fromResult, toResult, request.Amount);
                return Task.FromResult(Result<Account>.Success(toResult));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred transfering money from account {FromAccount} to account {ToAccount}",
                    request.FromAccountId, request.ToAccountId);
                return Task.FromResult(Result<Account>.Failure(ex));
            }
        }

        private Account ValidateAccount(Guid accountId)
        {
            var account = accountService.Get(accountId);
            if (account is null)
            {
                throw new Exception($"Account {accountId} does not exist");
            }

            return account;
        }

        private void PublishAccountEvent(Account fromAccount, Account toAccount, decimal amount)
        {
            accountEventService.Add(new AccountEvent
            {
                Id = Guid.NewGuid(),
                Name = EventTypes.MoneyTransferred,
                Payload = JsonConvert.SerializeObject((fromAccount, toAccount, amount))
            });
        }
    }
}
