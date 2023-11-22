using FluentAssertions;
using Moq;
using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.Accounts;
using Xunit;
using BankingSimulation.Domain.Events;

namespace BankingSimulation.Application.Test.Commands
{
    public class DepositMoneyCommandTest : RequestTestBase<DepositMoneyCommandHandler>
    {
        private readonly Mock<IAccountService> mockAccountService = new();

        private readonly Mock<IAccountEventService> mockAccountEventService = new();

        private readonly DepositMoneyCommandHandler handler;

        public DepositMoneyCommandTest()
        {
            handler = new DepositMoneyCommandHandler(mockAccountService.Object, mockAccountEventService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnFailureWhenAmountInvalid()
        {
            // Given

            // When
            var result = await handler.Handle(new DepositMoneyCommand
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
            var result = await handler.Handle(new DepositMoneyCommand(), CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldPublishEvent()
        {
            // Given
            var accountId = Guid.NewGuid();
            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).Returns((Guid id) => new Account
            {
                Id = id,
                Balance = 0,
                AccountType = AccountType.Checking
            });
            mockAccountService.Setup(x => x.Update(It.IsAny<Account>())).Returns((Account account) => account);

            // When
            var result = await handler.Handle(new DepositMoneyCommand { AccountId = accountId, Amount = 100 }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
            mockAccountEventService.Verify(x => x.Add(It.Is<AccountEvent>(l => l.Name == EventTypes.MoneyDeposited)));
        }
    }
}
