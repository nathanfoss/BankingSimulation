namespace BankingSimulation.Domain.Accounts
{
    public interface IAccountService
    {
        Task<Account> Get(Guid id);

        Task<Account> Add(Account account);

        Task<Account> Update(Account account);

        Task<IEnumerable<Account>> GetByAccountHolder(Guid accountHolderId);
    }
}