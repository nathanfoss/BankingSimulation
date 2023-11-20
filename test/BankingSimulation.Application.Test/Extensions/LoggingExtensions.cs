using Microsoft.Extensions.Logging;
using Moq;

namespace BankingSimulation.Application.Test.Extensions
{
    public static class LoggingExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> mockLogger, LogLevel level, string message)
        {
            mockLogger.Verify(l =>
                l.Log(
                    It.Is<LogLevel>(ll => ll == level),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().ToLower().Contains(message.ToLower())),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ));
        }
    }
}
