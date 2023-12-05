using BankingSimulation.Domain;
using BankingSimulation.Domain.AccountHolders;
using Microsoft.EntityFrameworkCore;

namespace BankingSimulation.Infrastructure.AccountHolders
{
    public class AccountHolderService : IAccountHolderService
    {
        private readonly BankDbContext context;

        public AccountHolderService(BankDbContext context)
        {
            this.context = context;
        }

        public async Task<AccountHolder> Add(AccountHolder accountHolder)
        {
            context.AccountHolders.Add(accountHolder);
            await context.SaveChangesAsync();
            return accountHolder;
        }

        public async Task<AccountHolder> Get(Guid id)
        {
            return await context.AccountHolders.FindAsync(id);
        }

        public async Task<AccountHolder> GetByPublicIdentifier(Guid publicIdentifier)
        {
            return await context.AccountHolders.FirstOrDefaultAsync(x => x.PublicIdentifier == publicIdentifier);
        }
    }
}
