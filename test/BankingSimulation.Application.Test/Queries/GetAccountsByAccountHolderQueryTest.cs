using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankingSimulation.Application.Test.Queries
{
    public class GetAccountsByAccountHolderQueryTest : RequestTestBase<GetAccountsByAccountHolderQueryHandler>
    {
        private readonly Mock<IAccountService> mockAccountService = new();

        public GetAccountsByAccountHolderQueryTest()
        {
            handler = new GetAccountsByAccountHolderQueryHandler(mockAccountService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnFailureIfExceptionThrown()
        {
            // Given
            mockAccountService.Setup(x => x.GetByAccountHolder(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            // When
            var result = await handler.Handle(new GetAccountsByAccountHolderQuery { AccountHolderPublicIdentifier = Guid.NewGuid() }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnSuccess()
        {
            // Given
            mockAccountService.Setup(x => x.GetByAccountHolder(It.IsAny<Guid>())).ReturnsAsync(new List<Account>());

            // When
            var result = await handler.Handle(new GetAccountsByAccountHolderQuery { AccountHolderPublicIdentifier = Guid.NewGuid() }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
        }
    }
}
