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

        public Task<Result<string>> Handle(AddAccountLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var events = accountEventService.GetAll();
                if (!events.Any())
                {
                    return Task.FromResult(Result<string>.Success("No events found"));
                }

                ProcessEvents(events);
                return Task.FromResult(Result<string>.Success("Events Handled"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred handling events");
                return Task.FromResult(Result<string>.Failure(ex));
            }
        }

        private void ProcessEvents(IEnumerable<AccountEvent> events)
        {
            var groups = events.GroupBy(e => e.Name);
            foreach (var group in groups)
            {
                switch (group.Key)
                {
                    case EventTypes.AccountCreated:
                        ProcessAccountCreatedEvents(group.Select(x => x));
                        break;
                    case EventTypes.AccountLinked:
                        ProcessAccountLinkedEvents(group.Select(x => x));
                        break;
                    case EventTypes.AccountReassigned:
                        ProcessAccountReassignedEvents(group.Select(x => x));
                        break;
                    case EventTypes.AccountClosed:
                        ProcessAccountClosedEvents(group.Select(x => x));
                        break;
                    case EventTypes.MoneyDeposited:
                        ProcessMoneyDepositedEvents(group.Select(x => x));
                        break;
                    case EventTypes.MoneyWithdrawn:
                        ProcessMoneyWithdrawnEvents(group.Select(x => x));
                        break;
                    case EventTypes.MoneyTransferred:
                        ProcessMoneyTransferredEvents(group.Select(x => x));
                        break;
                    default:
                        logger.LogWarning("Event Type {EventType} is unhandled and is being ignored", group.Key);
                        break;
                }
            }
        }

        private void ProcessMoneyWithdrawnEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var payload = JsonConvert.DeserializeObject<(Account, decimal)>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    AccountId = payload.Item1.Id,
                    CreatedDate = DateTime.UtcNow,
                    EventType = AccountEventType.Withdraw,
                    Metadata = new Dictionary<string, string>()
                    {
                        { "Amount", $"{payload.Item2}" },
                        { "Balance", $"{payload.Item1.Balance}" }
                    }
                };
            });

            accountLogService.Add(logs);
            accountEventService.Remove(events);
        }

        private void ProcessMoneyTransferredEvents(IEnumerable<AccountEvent> events)
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
                        EventType = AccountEventType.Transfer,
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
                        EventType = AccountEventType.Transfer,
                        Metadata = new Dictionary<string, string>()
                        {
                            { "FromAccount", $"{payload.Item1.Id}" },
                            { "Amount", $"{payload.Item3}" },
                            { "Balance", $"{payload.Item2.Balance}" }
                        }
                    }
                };
            }).SelectMany(x => x);

            accountLogService.Add(logs);
            accountEventService.Remove(events);
        }

        private void ProcessMoneyDepositedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var payload = JsonConvert.DeserializeObject<(Account, decimal)>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    AccountId = payload.Item1.Id,
                    CreatedDate = DateTime.UtcNow,
                    EventType = AccountEventType.Deposit,
                    Metadata = new Dictionary<string, string>()
                    {
                        { "Amount", $"{payload.Item2}" },
                        { "Balance", $"{payload.Item1.Balance}" }
                    }
                };
            });

            accountLogService.Add(logs);
            accountEventService.Remove(events);
        }

        private void ProcessAccountClosedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventType.Closed,
                    AccountId = account.Id,
                    Metadata = new Dictionary<string, string>()
                };
            });

            accountLogService.Add(logs);
            accountEventService.Remove(events);
        }

        private void ProcessAccountReassignedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventType.Reassigned,
                    AccountId = account.Id,
                    Metadata = new Dictionary<string, string>
                    {
                        { "AccountOwner", account.AccountHolder.PublicIdentifier.ToString() }
                    }
                };
            });

            accountLogService.Add(logs);
            accountEventService.Remove(events);
        }

        private void ProcessAccountCreatedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventType.Created,
                    AccountId = account.Id,
                    Metadata = new Dictionary<string, string>()
                };
            });

            accountLogService.Add(logs);
            accountEventService.Remove(events);
        }

        private void ProcessAccountLinkedEvents(IEnumerable<AccountEvent> events)
        {
            var logs = events.Select(e =>
            {
                var account = JsonConvert.DeserializeObject<Account>(e.Payload);
                return new AccountLog
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    EventType = AccountEventType.Linked,
                    AccountId = account.LinkedAccountId.Value,
                    Metadata = new Dictionary<string, string>
                    {
                        { "LinkedAccountId", account.Id.ToString() }
                    }
                };
            });

            accountLogService.Add(logs);
            accountEventService.Remove(events);
        }
    }
}
