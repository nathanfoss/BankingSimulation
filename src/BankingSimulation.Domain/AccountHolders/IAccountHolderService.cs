namespace BankingSimulation.Domain.AccountHolders
{
    public interface IAccountHolderService
    {
        AccountHolder Add(AccountHolder accountHolder);

        AccountHolder Get(Guid id);

        AccountHolder GetByPublicIdentifier(Guid publicIdentifier);
    }
}
