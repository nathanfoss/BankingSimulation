using BankingSimulation.Application.Models;
using BankingSimulation.Domain.AccountLogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BankingSimulation.Application.Queries
{
    public class GetAccountLogsQuery : IRequest<Result<IEnumerable<AccountLog>>>
    {
        public Guid AccountId { get; set; }
    }

    public class GetAccountLogsQueryHandler : IRequestHandler<GetAccountLogsQuery, Result<IEnumerable<AccountLog>>>
    {
        private readonly IAccountLogService accountLogService;

        private readonly ILogger<GetAccountLogsQueryHandler> logger;

        public GetAccountLogsQueryHandler(IAccountLogService accountLogService, ILogger<GetAccountLogsQueryHandler> logger)
        {
            this.accountLogService = accountLogService;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<AccountLog>>> Handle(GetAccountLogsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var logs = await accountLogService.GetByAccount(request.AccountId);
                return Result<IEnumerable<AccountLog>>.Success(logs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving logs for account {Account}", request.AccountId);
                return Result<IEnumerable<AccountLog>>.Failure(ex);
            }
        }
    }
}
