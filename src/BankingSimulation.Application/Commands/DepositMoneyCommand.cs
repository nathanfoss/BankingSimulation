﻿using MediatR;
using Microsoft.Extensions.Logging;
using BankingSimulation.Application.Models;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Events;
using Newtonsoft.Json;

namespace BankingSimulation.Application.Commands
{
    public class DepositMoneyCommand : IRequest<Result<Account>>
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }

    public class DepositMoneyCommandHandler : IRequestHandler<DepositMoneyCommand, Result<Account>>
    {
        private readonly IAccountService accountService;

        private readonly IAccountEventService accountEventService;

        private readonly ILogger<DepositMoneyCommandHandler> logger;

        public DepositMoneyCommandHandler(IAccountService accountService, IAccountEventService accountEventService, ILogger<DepositMoneyCommandHandler> logger)
        {
            this.accountService = accountService;
            this.accountEventService = accountEventService;
            this.logger = logger;
        }

        public Task<Result<Account>> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    throw new ArgumentException("Invalid deposit amount");
                }

                var account = accountService.Get(request.AccountId);
                if (account is null)
                {
                    throw new Exception($"Account {request.AccountId} does not exist");
                }

                account.Balance += request.Amount;
                var result = accountService.Update(account);
                PublishAccountEvent(result, request.Amount);
                return Task.FromResult(Result<Account>.Success(result));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred depositing money into account {account}", request.AccountId);
                return Task.FromResult(Result<Account>.Failure(ex));
            }
        }

        private void PublishAccountEvent(Account account, decimal amount)
        {
            accountEventService.Add(new AccountEvent
            {
                Id = Guid.NewGuid(),
                Name = EventTypes.MoneyDeposited,
                Payload = JsonConvert.SerializeObject((account, amount))
            });
        }
    }
}
