using BankingSimulation.Application.Models;
using BankingSimulation.Domain.Accounts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BankingSimulation.Application.Queries
{
    public class GetAccountsByAccountHolderQuery : IRequest<Result<IEnumerable<Account>>>
    {
        public Guid AccountHolderPublicIdentifier { get; set; }
    }

    public class GetAccountsByAccountHolderQueryHandler : IRequestHandler<GetAccountsByAccountHolderQuery, Result<IEnumerable<Account>>>
    {
        private readonly IAccountService accountService;

        private readonly ILogger<GetAccountsByAccountHolderQueryHandler> logger;

        public GetAccountsByAccountHolderQueryHandler(IAccountService accountService, ILogger<GetAccountsByAccountHolderQueryHandler> logger)
        {
            this.accountService = accountService;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<Account>>> Handle(GetAccountsByAccountHolderQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var accounts = await accountService.GetByAccountHolder(request.AccountHolderPublicIdentifier);
                return Result<IEnumerable<Account>>.Success(accounts);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error getting accounts for account holder {AccountHolder}", request.AccountHolderPublicIdentifier);
                return Result<IEnumerable<Account>>.Failure(ex);
            }
        }
    }
}
