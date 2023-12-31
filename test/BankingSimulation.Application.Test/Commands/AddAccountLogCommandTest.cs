﻿using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.Events;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace BankingSimulation.Application.Test.Commands
{
    public class AddAccountLogCommandTest : RequestTestBase<AddAccountLogCommandHandler>
    {
        private readonly Mock<IAccountEventService> mockAccountEventService = new();

        private readonly Mock<IAccountLogService> mockAccountLogService = new();

        public AddAccountLogCommandTest()
        {
            handler = new AddAccountLogCommandHandler(mockAccountEventService.Object, mockAccountLogService.Object, mockLogger.Object);
            var payload = JsonConvert.SerializeObject(new Account());
        }

        [Theory]
        [MemberData(nameof(GenerateTheoryData))]
        public async Task ShouldProcessEvents((List<AccountEvent>, AccountEventTypeEnum) testData)
        {
            // Given
            mockAccountEventService.Setup(x => x.GetAll()).ReturnsAsync(testData.Item1);

            // When
            var result = await handler.Handle(new AddAccountLogCommand(), CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountLogService.Verify(x => x.Add(It.Is<IEnumerable<AccountLog>>(l => l.All(log => log.EventTypeId == testData.Item2))));
            mockAccountEventService.Verify(x => x.Remove(It.IsAny<IEnumerable<AccountEvent>>()));
        }

        public static TheoryData<(List<AccountEvent>, AccountEventTypeEnum)> GenerateTheoryData =>
            new()
            {
                new()
                {
                    Item1 = new List<AccountEvent>
                    {
                        new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            Name = EventTypes.AccountCreated,
                            Payload = JsonConvert.SerializeObject(new Account())
                        }
                    },
                    Item2 = AccountEventTypeEnum.Created
                },
                new()
                {
                    Item1 = new List<AccountEvent>
                    {
                        new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            Name = EventTypes.AccountLinked,
                            Payload = JsonConvert.SerializeObject(new Account
                            {
                                LinkedAccountId = Guid.NewGuid()
                            })
                        }
                    },
                    Item2 = AccountEventTypeEnum.Linked
                },
                new()
                {
                    Item1 = new List<AccountEvent>
                    {
                        new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            Name = EventTypes.AccountReassigned,
                            Payload = JsonConvert.SerializeObject(new Account
                            {
                                AccountHolder = new Domain.AccountHolders.AccountHolder
                                {
                                    PublicIdentifier = Guid.NewGuid()
                                }
                            })
                        }
                    },
                    Item2 = AccountEventTypeEnum.Reassigned
                },
                new()
                {
                    Item1 = new List<AccountEvent>
                    {
                        new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            Name = EventTypes.AccountClosed,
                            Payload = JsonConvert.SerializeObject(new Account())
                        }
                    },
                    Item2 = AccountEventTypeEnum.Closed
                },
                new()
                {
                    Item1 = new List<AccountEvent>
                    {
                        new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            Name = EventTypes.MoneyDeposited,
                            Payload = JsonConvert.SerializeObject((new Account(), 5))
                        }
                    },
                    Item2 = AccountEventTypeEnum.Deposit
                },
                new()
                {
                    Item1 = new List<AccountEvent>
                    {
                        new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            Name = EventTypes.MoneyWithdrawn,
                            Payload = JsonConvert.SerializeObject((new Account(), 5))
                        }
                    },
                    Item2 = AccountEventTypeEnum.Withdraw
                },
                new()
                {
                    Item1 = new List<AccountEvent>
                    {
                        new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            Name = EventTypes.MoneyTransferred,
                            Payload = JsonConvert.SerializeObject((new Account(), new Account(), 5))
                        }
                    },
                    Item2 = AccountEventTypeEnum.Transfer
                }
            };
    }
}
