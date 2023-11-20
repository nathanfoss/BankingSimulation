using FluentAssertions;
using Moq;
using BankingSimulation.Application.Commands;
using BankingSimulation.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BankingSimulation.Application.Test.Commands
{
    public class DepositMoneyCommandTest : RequestTestBase<DepositMoneyCommandHandler>
    {
        private readonly Mock<IAccountService> mockAccountService;

        private readonly DepositMoneyCommandHandler handler;

        public DepositMoneyCommandTest()
        {
            mockAccountService = new();
            handler = new DepositMoneyCommandHandler(mockAccountService.Object, mockLogger.Object);
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
    }
}
