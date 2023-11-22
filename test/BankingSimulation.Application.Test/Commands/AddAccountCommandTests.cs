using FluentAssertions;
using Moq;
using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.Accounts;
using Xunit;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Events;

namespace BankingSimulation.Application.Test.Commands
{
    public class AddAccountCommandTests : RequestTestBase<AddAccountCommandHandler>
    {
        private readonly Mock<IAccountService> mockAccountService = new();

        private readonly Mock<IAccountHolderService> mockAccountHolderService = new();

        private readonly Mock<IAccountEventService> mockAccountEventService = new();

        private readonly AddAccountCommandHandler handler;

        public AddAccountCommandTests()
        {
            handler = new AddAccountCommandHandler(mockAccountService.Object, mockAccountHolderService.Object, mockAccountEventService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnFailureWhenCheckingAccountHasNoLinkedAccount()
        {
            // Given
            var account = new Account
            {
                AccountType = AccountType.Checking,
                LinkedAccountId = null
            };

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenCheckingAccountLinkedAccountInvalid()
        {
            // Given
            var account = new Account
            {
                AccountType = AccountType.Checking,
                LinkedAccountId = Guid.Empty
            };

            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).Returns(default(Account));

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenValidationFails()
        {
            // Given
            var account = new Account
            {
                AccountType = AccountType.Savings,
                AccountHolder = null
            };

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldAddAccountCreatedEvent()
        {
            // Given
            var account = new Account
            {
                AccountType = AccountType.Savings,
                AccountHolder = new AccountHolder
                {
                    Id = Guid.NewGuid(),
                    FullName = "Test Person",
                    PublicIdentifier = Guid.NewGuid()
                }
            };

            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).Returns((Account account) => account);

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            mockAccountEventService.Verify(x => x.Add(It.Is<IEnumerable<AccountEvent>>(l => l.All(e => e.Name == EventTypes.AccountCreated))));
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldAddAccountLinkedEventForCheckingAccount()
        {
            // Given
            var linkedAccountId = Guid.NewGuid();
            var account = new Account
            {
                AccountType = AccountType.Checking,
                AccountHolder = new AccountHolder
                {
                    Id = Guid.NewGuid(),
                    FullName = "Test Person",
                    PublicIdentifier = Guid.NewGuid()
                },
                LinkedAccountId = linkedAccountId
            };

            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).Returns((Account account) => account);
            mockAccountService.Setup(x => x.Get(It.Is<Guid>(i => i == linkedAccountId))).Returns(new Account { Id = linkedAccountId });

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            mockAccountEventService.Verify(x => x.Add(It.Is<IEnumerable<AccountEvent>>(l => l.Any(e => e.Name == EventTypes.AccountLinked))));
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldAddAccountHolderIfNotFound()
        {
            // Given
            var account = new Account
            {
                AccountType = AccountType.Savings,
                AccountHolder = new AccountHolder
                {
                    Id = Guid.NewGuid(),
                    FullName = "Test Person",
                    PublicIdentifier = Guid.NewGuid()
                }
            };

            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).Returns((Account account) => account);

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            mockAccountHolderService.Verify(x => x.Add(It.IsAny<AccountHolder>()));
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldAssignAccountHolderIfFound()
        {
            // Given
            var accountHolderId = Guid.NewGuid();
            var account = new Account
            {
                AccountType = AccountType.Savings,
                AccountHolder = new AccountHolder
                {
                    FullName = "Test Person",
                    PublicIdentifier = Guid.NewGuid()
                }
            };

            mockAccountHolderService.Setup(x => x.GetByPublicIdentifier(It.IsAny<Guid>())).Returns((Guid publicId) => new AccountHolder
            {
                Id = accountHolderId,
                FullName = "Some Person",
                PublicIdentifier = Guid.NewGuid()
            });
            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).Returns((Account account) => account);

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            mockAccountService.Verify(x => x.Add(It.Is<Account>(x => x.AccountHolderId == accountHolderId)));
            result.Succeeded.Should().BeTrue();
        }
    }
}
