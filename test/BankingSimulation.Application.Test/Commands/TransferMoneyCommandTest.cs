using FluentAssertions;
using Moq;
using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.Accounts;
using Xunit;
using BankingSimulation.Domain.AccountLogs;

namespace BankingSimulation.Application.Test.Commands
{
    public class TransferMoneyCommandTest : RequestTestBase<TransferMoneyCommandHandler>
    {
        private readonly Mock<IAccountService> mockAccountService = new();

        private readonly Mock<IAccountLogService> mockAccountLogService = new();

        private readonly TransferMoneyCommandHandler handler;

        public TransferMoneyCommandTest()
        {
            handler = new TransferMoneyCommandHandler(mockAccountService.Object, mockAccountLogService.Object, mockLogger.Object);
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
        public async Task ShouldReturnFailureWhenFromAccountNotExist()
        {
            // Given
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(fromAccountId)).Returns(default(Account));
            mockAccountService.Setup(x => x.Get(toAccountId)).Returns(new Account());

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
            mockAccountService.Setup(x => x.Get(fromAccountId)).Returns(new Account());
            mockAccountService.Setup(x => x.Get(toAccountId)).Returns(default(Account));

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
            mockAccountService.Setup(x => x.Get(fromAccountId)).Returns(new Account());
            mockAccountService.Setup(x => x.Get(toAccountId)).Returns(new Account());

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
        public async Task ShouldAddLog()
        {
            // Given
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(fromAccountId)).Returns((Guid id) => new Account
            {
                Id = id,
                Balance = 100,
                AccountType = AccountType.Checking
            });
            mockAccountService.Setup(x => x.Get(toAccountId)).Returns((Guid id) => new Account
            {
                Id = id,
                Balance = 100,
                AccountType = AccountType.Checking
            });
            mockAccountService.Setup(x => x.Update(It.IsAny<Account>())).Returns((Account account) => account);

            // When
            var result = await handler.Handle(new TransferMoneyCommand { FromAccountId = fromAccountId, ToAccountId = toAccountId, Amount = 50 }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountLogService.Verify(x => x.Add(It.Is<IEnumerable<AccountLog>>(l => l.Count() == 2)));
            mockAccountLogService.Verify(x => x.Add(It.Is<IEnumerable<AccountLog>>(l => l.All(x => x.EventType == AccountEventType.Transfer && x.Metadata.ContainsKey("Amount") && x.Metadata.ContainsKey("Balance")))));
        }
    }
}
