using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.Events;
using Newtonsoft.Json;
using BankingSimulation.Domain.AccountTypes;

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

        private readonly IAccountEventService accountEventService;

        private readonly ILogger<AddAccountCommandHandler> logger;

        public AddAccountCommandHandler(IAccountService accountService, IAccountHolderService accountHolderService,
            IAccountEventService accountEventService, ILogger<AddAccountCommandHandler> logger)
        {
            this.accountService = accountService;
            this.accountHolderService = accountHolderService;
            this.accountEventService = accountEventService;
            this.logger = logger;
        }

        public async Task<Result<Guid>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = request.Account;
                await ValidateLinkedAccount(account);
                await ValidateAccountHolder(account);
                var result = await accountService.Add(account);
                await PublishAccountEvent(result);
                return Result<Guid>.Success(result.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
                return Result<Guid>.Failure(ex);
            }
        }

        private async Task PublishAccountEvent(Account account)
        {
            var events = new List<AccountEvent>
            {
                new AccountEvent
                {
                    Id = Guid.NewGuid(),
                    Name = EventTypes.AccountCreated,
                    Payload = JsonConvert.SerializeObject(account)
                }
            };

            if (account.AccountTypeId == AccountTypeEnum.Checking)
            {
                events.Add(new AccountEvent
                {
                    Id = Guid.NewGuid(),
                    Name = EventTypes.AccountLinked,
                    Payload = JsonConvert.SerializeObject(account)
                });
            }

            await accountEventService.Add(events);
        }

        private async Task ValidateAccountHolder(Account account)
        {
            if (account.AccountHolder is null)
            {
                throw new Exception("Account holder should not be null");
            }

            var accountHolder = await accountHolderService.GetByPublicIdentifier(account.AccountHolder.PublicIdentifier);
            if (accountHolder is not null)
            {
                account.AccountHolderId = accountHolder.Id;
                return;
            }

            var holder = account.AccountHolder;
            holder.Id = Guid.NewGuid();
            await accountHolderService.Add(holder);
            account.AccountHolderId = holder.Id;
        }

        private async Task ValidateLinkedAccount(Account account)
        {
            if (account.AccountTypeId == AccountTypeEnum.Savings)
            {
                return;
            }

            var linkedAccount = await accountService.Get(account.LinkedAccountId.Value);
            if (linkedAccount == null)
            {
                throw new Exception($"Invalid linked account: {account.LinkedAccountId}");
            }
        }
    }
}
