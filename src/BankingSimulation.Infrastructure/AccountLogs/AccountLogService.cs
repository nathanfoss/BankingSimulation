using BankingSimulation.Domain.AccountLogs;

namespace BankingSimulation.Infrastructure.AccountLogs
{
    public class AccountLogService : IAccountLogService
    {
        private readonly List<AccountLog> accountLogs = new List<AccountLog>();

        public void Add(AccountLog accountLog)
        {
            accountLogs.Add(accountLog);
        }

        public IEnumerable<AccountLog> GetByAccount(Guid accountId)
        {
            return accountLogs.Where(x => x.AccountId == accountId);
        }
    }
}
