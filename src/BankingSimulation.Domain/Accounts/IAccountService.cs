namespace BankingSimulation.Domain.Accounts
{
    public interface IAccountService
    {
        Account Get(Guid id);

        Account Add(Account account);

        Account Update(Account account);

        IEnumerable<Account> GetByAccountHolder(Guid accountHolderId);
    }
}