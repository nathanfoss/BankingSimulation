using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.Accounts;

namespace BankingSimulation.Application.Commands
{
    public class AddAccountCommand : IRequest<Result<Guid>>
    {
        public Account Account { get; set; }
    }

    public class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, Result<Guid>>
    {
        private readonly IAccountService accountService;

        private readonly IAccountHolderService accountHolderService;

        private readonly ILogger<AddAccountCommandHandler> logger;

        public AddAccountCommandHandler(IAccountService accountService, IAccountHolderService accountHolderService, ILogger<AddAccountCommandHandler> logger)
        {
            this.accountService = accountService;
            this.accountHolderService = accountHolderService;
            this.logger = logger;
        }

        public Task<Result<Guid>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = request.Account;
                ValidateLinkedAccount(account);
                ValidateAccountHolder(account);
                var result = accountService.Add(account);
                return Task.FromResult(Result<Guid>.Success(result.Id));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
                return Task.FromResult(Result<Guid>.Failure(ex));
            }
        }

        private void ValidateAccountHolder(Account account)
        {
            if (account.AccountHolder is null)
            {
                throw new Exception("Account holder should not be null");
            }

            var accountHolder = accountHolderService.GetByPublicIdentifier(account.AccountHolder.PublicIdentifier);
            if (accountHolder is not null)
            {
                account.AccountHolderId = accountHolder.Id;
                return;
            }

            var holder = account.AccountHolder;
            holder.Id = Guid.NewGuid();
            accountHolderService.Add(holder);
            account.AccountHolderId = holder.Id;
        }

        private void ValidateLinkedAccount(Account account)
        {
            if (account.AccountType == AccountType.Savings)
            {
                return;
            }

            var linkedAccount = accountService.Get(account.LinkedAccountId.Value);
            if (linkedAccount == null)
            {
                throw new Exception($"Invalid linked account: {account.LinkedAccountId}");
            }
        }
    }
}
