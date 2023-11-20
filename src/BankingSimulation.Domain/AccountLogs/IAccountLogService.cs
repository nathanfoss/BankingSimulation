namespace BankingSimulation.Domain.AccountLogs
{
    public interface IAccountLogService
    {
        void Add(AccountLog accountLog);

        IEnumerable<AccountLog> GetByAccount(Guid accountId);
    }
}
