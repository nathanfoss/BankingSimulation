using BankingSimulation.Domain.Events;

namespace BankingSimulation.Infrastructure.Events
{
    public class AccountEventService : IAccountEventService
    {
        private readonly List<AccountEvent> accountEvents = new();

        public void Add(AccountEvent accountEvent)
        {
            accountEvents.Add(accountEvent);
        }

        public void Add(IEnumerable<AccountEvent> accountEvents)
        {
            this.accountEvents.AddRange(accountEvents);
        }

        public IEnumerable<AccountEvent> GetAll()
        {
            return accountEvents;
        }

        public void Remove(AccountEvent accountEvent)
        {
            accountEvents.Remove(accountEvent);
        }

        public void Remove(IEnumerable<AccountEvent> accountEvents)
        {
            foreach (var accountEvent in accountEvents)
            {
                Remove(accountEvent);
            }
        }
    }
}
