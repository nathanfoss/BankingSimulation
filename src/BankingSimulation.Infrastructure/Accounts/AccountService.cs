using BankingSimulation.Domain;
using BankingSimulation.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace BankingSimulation.Infrastructure.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly BankDbContext context;

        public AccountService(BankDbContext context)
        {
            this.context = context;
        }

        public async Task<Account> Get(Guid id)
        {
            return await context.Accounts.FindAsync(id);
        }

        public async Task<Account> Add(Account account)
        {
            var added = context.Accounts.Add(account);
            await context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<Account> Update(Account account)
        {
            var accountToUpdate = await Get(account.Id);
            accountToUpdate.Balance = account.Balance;
            accountToUpdate.AccountHolderId = account.AccountHolderId;
            accountToUpdate.AccountHolder = account.AccountHolder;
            accountToUpdate.LinkedAccountId = account.LinkedAccountId;

            await context.SaveChangesAsync();
            return accountToUpdate;
        }

        public async Task<IEnumerable<Account>> GetByAccountHolder(Guid accountHolderId)
        {
            return await context.Accounts.Where(x => x.AccountHolderId == accountHolderId).ToListAsync();
        }
    }
}