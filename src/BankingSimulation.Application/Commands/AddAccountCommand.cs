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
        public NewAccountViewModel Account { get; set; }
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
                var holderId = await ValidateAccountHolder(account);
                var result = await accountService.Add(new Account
                {
                    AccountHolderId = holderId,
                    AccountTypeId = account.AccountTypeId,
                    LinkedAccountId = account.LinkedAccountId
                });
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

        private async Task<Guid> ValidateAccountHolder(NewAccountViewModel account)
        {
            if (string.IsNullOrWhiteSpace(account.AccountHolderName))
            {
                throw new Exception("Account holder should not be null");
            }

            var accountHolder = await accountHolderService.GetByPublicIdentifier(account.AccountHolderPublicIdentifier);
            if (accountHolder is not null)
            {
                return accountHolder.Id;
            }

            var added = await accountHolderService.Add(new AccountHolder
            {
                FullName = account.AccountHolderName,
                PublicIdentifier = account.AccountHolderPublicIdentifier
            });
            return added.Id;
        }

        private async Task ValidateLinkedAccount(NewAccountViewModel account)
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
