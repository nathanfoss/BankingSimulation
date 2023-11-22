namespace BankingSimulation.Domain.Events
{
    public interface IAccountEventService
    {
        void Add(AccountEvent accountEvent);

        void Add(IEnumerable<AccountEvent> accountEvents);

        IEnumerable<AccountEvent> GetAll();

        void Remove(AccountEvent accountEvent);

        void Remove(IEnumerable<AccountEvent> accountEvents);
    }
}
