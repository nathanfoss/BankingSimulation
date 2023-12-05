using BankingSimulation.Application.Models;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BankingSimulation.Application.Commands
{
    public class AddAccountLogCommand : IRequest<Result<string>>
    {
    }

    public class AddAccountLogCommandHandler : IRequestHandler<AddAccountLogCommand, Result<string>>
    {
        private readonly IAccountEventService accountEventService;

        private readonly IAccountLogService accountLogService;

        private readonly ILogger<AddAccountLogCommandHandler> logger;

        public AddAccountLogCommandHandler(IAccountEventService accountEventService, IAccountLogService accountLogService, ILogger<AddAccountLogCommandHandler> logger)
        {
            this.accountEventService = accountEventService;
            this.accountLogService = accountLogService;
            this.logger = logger;
        }

        public async Task<Result<string>> Handle(AddAccountLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var events = await accountEventService.GetAll();
                if (!events.Any())
                {
                    return Result<string>.Success("No events found");
                }

                await ProcessEvents(events);
                return Result<string>.Success("Events Handled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred handling events");
                return Result<string>.Failure(ex);
            }
        }

        private async Task ProcessEvents(IEnumerable<AccountEvent> events)
        {
            var groups = events.GroupBy(e => e.Name);
            foreach (var group in groups)
            {
                switch (group.Key)
                {
                    case EventTypes.AccountCreated:
                        await ProcessAccountCreatedEvents(group.Select(x => x));
                        break;
                    case EventTypes.AccountLinked:
                        await ProcessAccountLinkedEvents(group.Select(x => x));
                        break;
                    case EventTypes.AccountReassigned:
                        await ProcessAccountReassignedEvents(group.Select(x => x));
                        break;
                    case EventTypes.AccountClosed:
                        await ProcessAccountClosedEvents(group.Select(x => x));
                        break;
                    case EventTypes.MoneyDeposited:
                        await ProcessMoneyDepositedEvents(group.Select(x => x));
                        break;
                    case EventTypes.MoneyWithdrawn:
                        await ProcessMoneyWithdrawnEvents(group.Select(x => x));
                        break;
                    case EventTypes.MoneyTransferred:
                        await ProcessMoneyTransferredEvents(group.Select(x => x));
                        break;
                    default:
                        logger.LogWarning("Event Type {EventType} is unhandled and is being ignored", group.Key);
                        break;
                }
            }
        }

        private async Task ProcessMoneyWithdrawnEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var payload = JsonConvert.DeserializeObject<(Account, decimal)>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    AccountId = payload.Item1.Id,
                    CreatedDate = DateTime.UtcNow,
                    EventType = AccountEventTypeEnum.Withdraw,
                    Metadata = new Dictionary<string, string>()
                    {
                        { "Amount", $"{payload.Item2}" },
                        { "Balance", $"{payload.Item1.Balance}" }
                    }
                };
            });

            await accountLogService.Add(logs);
            await accountEventService.Remove(events);
        }

        private async Task ProcessMoneyTransferredEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var payload = JsonConvert.DeserializeObject<(Account, Account, decimal)>(e.Payload);
                return new List<AccountLog>
                {
                    new AccountLog
                    {
                        Id = Guid.NewGuid(),
                        AccountId = payload.Item1.Id,
                        CreatedDate = DateTime.UtcNow,
                        EventType = AccountEventTypeEnum.Transfer,
                        Metadata = new Dictionary<string, string>()
                        {
                            { "ToAccount", $"{payload.Item2.Id}" },
                            { "Amount", $"{payload.Item3}" },
                            { "Balance", $"{payload.Item1.Balance}" }
                        }
                    },
                    new AccountLog
                    {
                        Id = Guid.NewGuid(),
                        AccountId = payload.Item2.Id,
                        CreatedDate = DateTime.UtcNow,
                        EventType = AccountEventTypeEnum.Transfer,
                        Metadata = new Dictionary<string, string>()
                        {
                            { "FromAccount", $"{payload.Item1.Id}" },
                            { "Amount", $"{payload.Item3}" },
                            { "Balance", $"{payload.Item2.Balance}" }
                        }
                    }
                };
            }).SelectMany(x => x);

            await accountLogService.Add(logs);
            await accountEventService.Remove(events);
        }

        private async Task ProcessMoneyDepositedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var payload = JsonConvert.DeserializeObject<(Account, decimal)>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    AccountId = payload.Item1.Id,
                    CreatedDate = DateTime.UtcNow,
                    EventType = AccountEventTypeEnum.Deposit,
                    Metadata = new Dictionary<string, string>()
                    {
                        { "Amount", $"{payload.Item2}" },
                        { "Balance", $"{payload.Item1.Balance}" }
                    }
                };
            });

            await accountLogService.Add(logs);
            await accountEventService.Remove(events);
        }

        private async Task ProcessAccountClosedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventTypeEnum.Closed,
                    AccountId = account.Id,
                    Metadata = new Dictionary<string, string>()
                };
            });

            await accountLogService.Add(logs);
            await accountEventService.Remove(events);
        }

        private async Task ProcessAccountReassignedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventTypeEnum.Reassigned,
                    AccountId = account.Id,
                    Metadata = new Dictionary<string, string>
                    {
                        { "AccountOwner", account.AccountHolder.PublicIdentifier.ToString() }
                    }
                };
            });

            await accountLogService.Add(logs);
            await accountEventService.Remove(events);
        }

        private async Task ProcessAccountCreatedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventTypeEnum.Created,
                    AccountId = account.Id,
                    Metadata = new Dictionary<string, string>()
                };
            });

            await accountLogService.Add(logs);
            await accountEventService.Remove(events);
        }

        private async Task ProcessAccountLinkedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventTypeEnum.Linked,
                    AccountId = account.LinkedAccountId.Value,
                    Metadata = new Dictionary<string, string>
                    {
                        { "LinkedAccountId", account.Id.ToString() }
                    }
                };
            });

            await accountLogService.Add(logs);
            await accountEventService.Remove(events);
        }
    }
}
