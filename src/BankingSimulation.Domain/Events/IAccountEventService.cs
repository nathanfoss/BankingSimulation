namespace BankingSimulation.Domain.Events
{
    public interface IAccountEventService
    {
        Task Add(AccountEvent accountEvent);

        Task Add(IEnumerable<AccountEvent> accountEvents);

        Task<IEnumerable<AccountEvent>> GetAll();

        Task Remove(AccountEvent accountEvent);

        Task Remove(IEnumerable<AccountEvent> accountEvents);
    }
}
