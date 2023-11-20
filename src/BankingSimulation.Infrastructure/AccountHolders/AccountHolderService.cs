using BankingSimulation.Domain.AccountHolders;

namespace BankingSimulation.Infrastructure.AccountHolders
{
    public class AccountHolderService : IAccountHolderService
    {
        private List<AccountHolder> _accountHolders;

        public AccountHolderService()
        {
            _accountHolders = new List<AccountHolder>();
        }

        public AccountHolder Add(AccountHolder accountHolder)
        {
            _accountHolders.Add(accountHolder);
            return accountHolder;
        }

        public AccountHolder Get(Guid id)
        {
            return _accountHolders.FirstOrDefault(x => x.Id == id);
        }

        public AccountHolder GetByPublicIdentifier(Guid publicIdentifier)
        {
            return _accountHolders.FirstOrDefault(x => x.PublicIdentifier == publicIdentifier);
        }
    }
}
