using BankingSimulation.Domain;
using BankingSimulation.Domain.AccountLogs;
using Microsoft.EntityFrameworkCore;

namespace BankingSimulation.Infrastructure.AccountLogs
{
    public class AccountLogService : IAccountLogService
    {
        private readonly BankDbContext context;

        public AccountLogService(BankDbContext context)
        {
            this.context = context;
        }

        public async Task Add(AccountLog accountLog)
        {
            context.AccountLogs.Add(accountLog);
            await context.SaveChangesAsync();
        }

        public async Task Add(IEnumerable<AccountLog> accountLogs)
        {
            context.AccountLogs.AddRange(accountLogs);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountLog>> GetByAccount(Guid accountId)
        {
            return await context.AccountLogs
                .Include(x => x.EventType)
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.AccountId == accountId)
                .ToListAsync();
        }
    }
}
