using FluentAssertions;
using Moq;
using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.Accounts;
using Xunit;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Events;
using BankingSimulation.Domain.AccountTypes;
using BankingSimulation.Application.Models;

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
            var account = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Checking,
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
            var account = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Checking,
                LinkedAccountId = Guid.Empty
            };

            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(default(Account));

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenValidationFails()
        {
            // Given
            var account = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Savings,
                AccountHolderName = null
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
            var account = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Savings,
                AccountHolderName = "Test Person",
                AccountHolderPublicIdentifier = Guid.NewGuid()
            };

            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).ReturnsAsync((Account account) => account);
            mockAccountHolderService.Setup(x => x.Add(It.IsAny<AccountHolder>())).ReturnsAsync((AccountHolder accountHolder) => accountHolder);

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountEventService.Verify(x => x.Add(It.Is<IEnumerable<AccountEvent>>(l => l.All(e => e.Name == EventTypes.AccountCreated))));
        }

        [Fact]
        public async Task ShouldAddAccountLinkedEventForCheckingAccount()
        {
            // Given
            var linkedAccountId = Guid.NewGuid();
            var account = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Checking,
                AccountHolderName = "Test Person",
                AccountHolderPublicIdentifier = Guid.NewGuid(),
                LinkedAccountId = linkedAccountId
            };

            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).ReturnsAsync((Account account) => account);
            mockAccountService.Setup(x => x.Get(It.Is<Guid>(i => i == linkedAccountId))).ReturnsAsync(new Account { Id = linkedAccountId });
            mockAccountHolderService.Setup(x => x.Add(It.IsAny<AccountHolder>())).ReturnsAsync((AccountHolder accountHolder) => accountHolder);

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountEventService.Verify(x => x.Add(It.Is<IEnumerable<AccountEvent>>(l => l.Any(e => e.Name == EventTypes.AccountLinked))));
        }

        [Fact]
        public async Task ShouldAddAccountHolderIfNotFound()
        {
            // Given
            var account = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Savings,
                AccountHolderName = "Test Person",
                AccountHolderPublicIdentifier = Guid.NewGuid()
            };

            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).ReturnsAsync((Account account) => account);
            mockAccountHolderService.Setup(x => x.Add(It.IsAny<AccountHolder>())).ReturnsAsync((AccountHolder accountHolder) => accountHolder);

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountHolderService.Verify(x => x.Add(It.IsAny<AccountHolder>()));
        }

        [Fact]
        public async Task ShouldAssignAccountHolderIfFound()
        {
            // Given
            var accountHolderId = Guid.NewGuid();
            var account = new NewAccountViewModel
            {
                AccountTypeId = AccountTypeEnum.Savings,
                AccountHolderName = "Test Person",
                AccountHolderPublicIdentifier = Guid.NewGuid()
            };

            mockAccountHolderService.Setup(x => x.GetByPublicIdentifier(It.IsAny<Guid>())).ReturnsAsync((Guid publicId) => new AccountHolder
            {
                Id = accountHolderId,
                FullName = "Some Person",
                PublicIdentifier = Guid.NewGuid()
            });
            mockAccountService.Setup(x => x.Add(It.IsAny<Account>())).ReturnsAsync((Account account) => account);

            // When
            var result = await handler.Handle(new AddAccountCommand { Account = account }, CancellationToken.None);

            // Then
            mockAccountService.Verify(x => x.Add(It.Is<Account>(x => x.AccountHolderId == accountHolderId)));
            result.Succeeded.Should().BeTrue();
        }
    }
}
