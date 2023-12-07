using FluentAssertions;
using Moq;
using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.Accounts;
using Xunit;
using BankingSimulation.Domain.Events;
using BankingSimulation.Domain.AccountTypes;

namespace BankingSimulation.Application.Test.Commands
{
    public class TransferMoneyCommandTest : RequestTestBase<TransferMoneyCommandHandler>
    {
        private readonly Mock<IAccountService> mockAccountService = new();

        private readonly Mock<IAccountEventService> mockAccountEventService = new();

        private readonly TransferMoneyCommandHandler handler;

        public TransferMoneyCommandTest()
        {
            handler = new TransferMoneyCommandHandler(mockAccountService.Object, mockAccountEventService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnFailureWhenAmountInvalid()
        {
            // Given

            // When
            var result = await handler.Handle(new TransferMoneyCommand
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = -100
            }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenAccountsInvalid()
        {
            // Given
            var accountId = Guid.NewGuid();

            // When
            var result = await handler.Handle(new TransferMoneyCommand
            {
                FromAccountId = accountId,
                ToAccountId = accountId,
                Amount = 100
            }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenFromAccountNotExist()
        {
            // Given
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(fromAccountId)).ReturnsAsync(default(Account));
            mockAccountService.Setup(x => x.Get(toAccountId)).ReturnsAsync(new Account());

            // When
            var result = await handler.Handle(new TransferMoneyCommand
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = 100
            }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenToAccountNotExist()
        {
            // Given
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(fromAccountId)).ReturnsAsync(new Account());
            mockAccountService.Setup(x => x.Get(toAccountId)).ReturnsAsync(default(Account));

            // When
            var result = await handler.Handle(new TransferMoneyCommand
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = 100
            }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenFromAccountInsufficientFunds()
        {
            // Given
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(fromAccountId)).ReturnsAsync(new Account());
            mockAccountService.Setup(x => x.Get(toAccountId)).ReturnsAsync(new Account());

            // When
            var result = await handler.Handle(new TransferMoneyCommand
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = 100
            }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldPublishEvent()
        {
            // Given
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(fromAccountId)).ReturnsAsync((Guid id) => new Account
            {
                Id = id,
                Balance = 100,
                AccountTypeId = AccountTypeEnum.Checking
            });
            mockAccountService.Setup(x => x.Get(toAccountId)).ReturnsAsync((Guid id) => new Account
            {
                Id = id,
                Balance = 100,
                AccountTypeId = AccountTypeEnum.Checking
            });
            mockAccountService.Setup(x => x.Update(It.IsAny<Account>())).ReturnsAsync((Account account) => account);

            // When
            var result = await handler.Handle(new TransferMoneyCommand { FromAccountId = fromAccountId, ToAccountId = toAccountId, Amount = 50 }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountEventService.Verify(x => x.Add(It.Is<AccountEvent>(l => l.Name == EventTypes.MoneyTransferred)));
        }
    }
}
