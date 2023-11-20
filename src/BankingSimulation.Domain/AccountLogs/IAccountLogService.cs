namespace BankingSimulation.Domain.AccountLogs
{
    public interface IAccountLogService
    {
        void Add(AccountLog accountLog);

        void Add(IEnumerable<AccountLog> accountLogs);

        IEnumerable<AccountLog> GetByAccount(Guid accountId);
    }
}
