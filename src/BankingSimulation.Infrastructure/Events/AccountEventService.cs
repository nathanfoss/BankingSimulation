using BankingSimulation.Domain;
using BankingSimulation.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace BankingSimulation.Infrastructure.Events
{
    public class AccountEventService : IAccountEventService
    {
        private readonly BankDbContext context;

        public AccountEventService(BankDbContext context)
        {
            this.context = context;
        }

        public async Task Add(AccountEvent accountEvent)
        {
            context.AccountEvents.Add(accountEvent);
            await context.SaveChangesAsync();
        }

        public async Task Add(IEnumerable<AccountEvent> accountEvents)
        {
            context.AccountEvents.AddRange(accountEvents);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountEvent>> GetAll()
        {
            return await context.AccountEvents.ToListAsync();
        }

        public async Task Remove(AccountEvent accountEvent)
        {
            context.AccountEvents.Remove(accountEvent);
            await context.SaveChangesAsync();
        }

        public async Task Remove(IEnumerable<AccountEvent> accountEvents)
        {
            context.AccountEvents.RemoveRange(accountEvents);
            await context.SaveChangesAsync();
        }
    }
}
