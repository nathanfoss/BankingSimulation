using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using BankingSimulation.Application.Queries;
using BankingSimulation.Application.Test.Extensions;
using BankingSimulation.Domain.Accounts;
using Xunit;

namespace BankingSimulation.Application.Test.Queries
{
    public class GetAccountQueryTest : RequestTestBase<GetAccountQueryHandler>
    {
        private readonly GetAccountQueryHandler target;

        private readonly Mock<IAccountService> mockAccountService;

        public GetAccountQueryTest()
        {
            mockAccountService = new();
            target = new GetAccountQueryHandler(mockAccountService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnSuccess()
        {
            // Given
            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).Returns(new Account());

            // When
            var result = await target.Handle(new GetAccountQuery(), CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenErrorThrown()
        {
            // Given
            mockAccountService.Setup(x => x.Get(It.IsAny<Guid>())).Throws(new Exception("Test"));

            // When
            var result = await target.Handle(new GetAccountQuery(), CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
            mockLogger.VerifyLog(LogLevel.Error, "unexpected error");
        }
    }
}
