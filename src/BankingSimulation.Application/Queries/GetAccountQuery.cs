﻿using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Domain.Accounts;

namespace BankingSimulation.Application.Queries
{
    public class GetAccountQuery : IRequest<Result<Account>>
    {
        public Guid AccountId { get; set; }
    }

    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Result<Account>>
    {
        private readonly IAccountService accountService;

        private readonly ILogger<GetAccountQueryHandler> logger;

        public GetAccountQueryHandler(IAccountService accountService,
            ILogger<GetAccountQueryHandler> logger)
        {
            this.accountService = accountService;
            this.logger = logger;
        }

        public async Task<Result<Account>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await accountService.Get(request.AccountId);
                return Result<Account>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
                return Result<Account>.Failure(ex);
            }
        }
    }
}
