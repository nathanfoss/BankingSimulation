using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankingSimulation.Application.Test
{
    public class RequestTestBase<T>
    {
        public readonly Mock<ILogger<T>> mockLogger;

        public readonly Mock<IConfiguration> mockConfiguration;

        public RequestTestBase()
        {
            mockLogger = new();
            mockConfiguration = new();
        }
    }
}
