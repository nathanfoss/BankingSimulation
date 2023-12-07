using BankingSimulation.Application.Models;
using BankingSimulation.Domain;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.AccountTypes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSimulation.Application.Commands
{
    public class SeedEnumsCommand : IRequest<Result<bool>>
    {
    }

    public class SeedEnumsCommandHandler : IRequestHandler<SeedEnumsCommand, Result<bool>>
    {
        private readonly BankDbContext context;

        public SeedEnumsCommandHandler(BankDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<bool>> Handle(SeedEnumsCommand request, CancellationToken cancellationToken)
        {
            var accountTypes = Enum.GetValues<AccountTypeEnum>().Select(id => new AccountType
            {
                Id = id,
                Name = id.ToString()
            });

            context.AccountTypes.AddRange(accountTypes);

            var eventTypes = Enum.GetValues<AccountEventTypeEnum>().Select(id => new AccountEventType
            {
                Id = id,
                Name = id.ToString()
            });

            context.AccountEventTypes.AddRange(eventTypes);

            await context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
