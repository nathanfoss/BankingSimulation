using BankingSimulation.Domain.AccountLogs;

namespace BankingSimulation.Infrastructure.AccountLogs
{
    public class AccountLogService : IAccountLogService
    {
        private readonly List<AccountLog> accountLogs = new();

        public void Add(AccountLog accountLog)
        {
            accountLogs.Add(accountLog);
        }

        public void Add(IEnumerable<AccountLog> accountLogs)
        {
            this.accountLogs.AddRange(accountLogs);
        }

        public IEnumerable<AccountLog> GetByAccount(Guid accountId)
        {
            return accountLogs.Where(x => x.AccountId == accountId);
        }
    }
}
