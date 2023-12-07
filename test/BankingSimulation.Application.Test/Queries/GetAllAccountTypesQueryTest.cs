using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountTypes;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankingSimulation.Application.Test.Queries
{
    public class GetAllAccountTypesQueryTest : RequestTestBase<GetAllAccountTypesQueryHandler>
    {
        private readonly Mock<IAccountTypeService> mockAccountTypeService = new();

        public GetAllAccountTypesQueryTest()
        {
            handler = new GetAllAccountTypesQueryHandler(mockAccountTypeService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnFailureIfExceptionThrown()
        {
            // Given
            mockAccountTypeService.Setup(x => x.GetAll()).ThrowsAsync(new Exception());

            // When
            var result = await handler.Handle(new GetAllAccountTypesQuery(), CancellationToken.None);

            // Then
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnSuccess()
        {
            // Given
            mockAccountTypeService.Setup(x => x.GetAll()).ReturnsAsync(new List<AccountType>());

            // When
            var result = await handler.Handle(new GetAllAccountTypesQuery(), CancellationToken.None);

            // Then
            result.Succeeded.Should().BeTrue();
        }
    }
}
