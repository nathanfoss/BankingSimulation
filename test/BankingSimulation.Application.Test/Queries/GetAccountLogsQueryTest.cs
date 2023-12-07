using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountLogs;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankingSimulation.Application.Test.Queries
{
    public class GetAccountLogsQueryTest : RequestTestBase<GetAccountLogsQueryHandler>
    {
        private readonly Mock<IAccountLogService> mockAccountLogService = new();

        public GetAccountLogsQueryTest()
        {
            handler = new GetAccountLogsQueryHandler(mockAccountLogService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnFailureIfExceptionThrown()
        {
            // Given
            mockAccountLogService.Setup(x => x.GetByAccount(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            // When
            var result = await handler.Handle(new GetAccountLogsQuery { AccountId = Guid.NewGuid() }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnSuccess()
        {
            // Given
            mockAccountLogService.Setup(x => x.GetByAccount(It.IsAny<Guid>())).ReturnsAsync(new List<AccountLog>());

            // When
            var result = await handler.Handle(new GetAccountLogsQuery { AccountId = Guid.NewGuid() }, CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
        }
    }
}
