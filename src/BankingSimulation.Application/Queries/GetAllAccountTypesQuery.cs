using BankingSimulation.Application.Models;
using BankingSimulation.Domain.AccountTypes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BankingSimulation.Application.Queries
{
    public class GetAllAccountTypesQuery : IRequest<Result<IEnumerable<AccountType>>>
    {
    }

    public class GetAllAccountTypesQueryHandler : IRequestHandler<GetAllAccountTypesQuery, Result<IEnumerable<AccountType>>>
    {
        private readonly IAccountTypeService accountTypeService;

        private readonly ILogger<GetAllAccountTypesQueryHandler> logger;

        public GetAllAccountTypesQueryHandler(IAccountTypeService accountTypeService, ILogger<GetAllAccountTypesQueryHandler> logger)
        {
            this.accountTypeService = accountTypeService;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<AccountType>>> Handle(GetAllAccountTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var accountTypes = await accountTypeService.GetAll();
                return Result<IEnumerable<AccountType>>.Success(accountTypes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error getting all account types");
                return Result<IEnumerable<AccountType>>.Failure(ex);
            }
        }
    }
}
