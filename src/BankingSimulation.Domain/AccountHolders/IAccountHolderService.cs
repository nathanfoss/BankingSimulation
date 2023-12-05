namespace BankingSimulation.Domain.AccountHolders
{
    public interface IAccountHolderService
    {
        Task<AccountHolder> Add(AccountHolder accountHolder);

        Task<AccountHolder> Get(Guid id);

        Task<AccountHolder> GetByPublicIdentifier(Guid publicIdentifier);
    }
}
