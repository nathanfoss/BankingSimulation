using BankingSimulation.Application.Models;
using BankingSimulation.Domain.AccountHolders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BankingSimulation.Application.Queries
{
    public class GetAccountHolderByIdentifierQuery : IRequest<Result<AccountHolder>>
    {
        public Guid PublicIdentifier { get; set; }
    }

    public class GetAccountHolderByIdentifierQueryHandler : IRequestHandler<GetAccountHolderByIdentifierQuery, Result<AccountHolder>>
    {
        private readonly IAccountHolderService accountHolderService;

        private readonly ILogger<GetAccountHolderByIdentifierQueryHandler> logger;

        public GetAccountHolderByIdentifierQueryHandler(IAccountHolderService accountHolderService, ILogger<GetAccountHolderByIdentifierQueryHandler> logger)
        {
            this.accountHolderService = accountHolderService;
            this.logger = logger;
        }

        public async Task<Result<AccountHolder>> Handle(GetAccountHolderByIdentifierQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var accountHolder = await accountHolderService.GetByPublicIdentifier(request.PublicIdentifier);
                return Result<AccountHolder>.Success(accountHolder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error getting account holder by identifier {Identifier}", request.PublicIdentifier);
                return Result<AccountHolder>.Failure(ex);
            }
        }
    }
}
