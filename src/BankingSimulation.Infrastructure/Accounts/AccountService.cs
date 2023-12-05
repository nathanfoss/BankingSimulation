using BankingSimulation.Domain.Accounts;

namespace BankingSimulation.Infrastructure.Accounts
{
    public class AccountService : IAccountService
    {
        private List<Account> _accounts;

        public AccountService()
        {
            _accounts = new List<Account>();
        }

        public Account Get(Guid id)
        {
            return _accounts.FirstOrDefault(a => a.Id == id);
        }

        public Account Add(Account account)
        {
            account.Id = Guid.NewGuid();
            _accounts.Add(account);
            return account;
        }

        public Account Update(Account account)
        {
            var accountToUpdate = _accounts.First(x => x.Id == account.Id);
            accountToUpdate.Balance = account.Balance;
            accountToUpdate.AccountHolderId = account.AccountHolderId;
            accountToUpdate.AccountHolder = account.AccountHolder;
            accountToUpdate.LinkedAccountId = account.LinkedAccountId;
            return accountToUpdate;
        }

        public IEnumerable<Account> GetByAccountHolder(Guid accountHolderId)
        {
            return _accounts.Where(x => x.AccountHolderId == accountHolderId);
        }
    }
}