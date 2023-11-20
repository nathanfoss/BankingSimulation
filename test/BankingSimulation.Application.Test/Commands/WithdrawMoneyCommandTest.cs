using FluentAssertions;
using Moq;
using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.Accounts;
using Xunit;
using BankingSimulation.Domain.AccountLogs;

namespace BankingSimulation.Application.Test.Commands
{
    public class WithdrawMoneyCommandTest : RequestTestBase<WithdrawMoneyCommandHandler>
    {
        private readonly Mock<IAccountService> mockAccountService = new();

        private readonly Mock<IAccountLogService> mockAccountLogService = new();

        private readonly WithdrawMoneyCommandHandler handler;

        public WithdrawMoneyCommandTest()
        {
            handler = new WithdrawMoneyCommandHandler(mockAccountService.Object, mockAccountLogService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnFailureWhenAmountInvalid()
        {
            // Given

            // When
            var result = await handler.Handle(new WithdrawMoneyCommand
            {
                AccountId = Guid.NewGuid(),
                Amount = -100
            }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenAccountNotExist()
        {
            // Given
            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).Returns(default(Account));

            // When
            var result = await handler.Handle(new WithdrawMoneyCommand(), CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenAccountHasInsufficientFunds()
        {
            // Given
            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).Returns((Guid id) => new Account
            {
                Id = id,
                AccountType = AccountType.Savings,
                Balance = 5
            });

            // When
            var result = await handler.Handle(new WithdrawMoneyCommand
            {
                AccountId = Guid.NewGuid(),
                Amount = 100
            }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldAddLog()
        {
            // Given
            var accountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).Returns((Guid id) => new Account
            {
                Id = id,
                Balance = 100,
                AccountType = AccountType.Checking
            });
            mockAccountService.Setup(x => x.Update(It.IsAny<Account>())).Returns((Account account) => account);

            // When
            var result = await handler.Handle(new WithdrawMoneyCommand { AccountId = accountId, Amount = 10 }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountLogService.Verify(x => x.Add(It.Is<AccountLog>(l => l.EventType == AccountEventType.Withdraw && l.Metadata.ContainsKey("Amount") && l.Metadata.ContainsKey("Balance"))));
        }
    }
}
