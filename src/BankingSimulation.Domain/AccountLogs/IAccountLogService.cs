namespace BankingSimulation.Domain.AccountLogs
{
    public interface IAccountLogService
    {
        Task Add(AccountLog accountLog);

        Task Add(IEnumerable<AccountLog> accountLogs);

        Task<IEnumerable<AccountLog>> GetByAccount(Guid accountId);
    }
}
