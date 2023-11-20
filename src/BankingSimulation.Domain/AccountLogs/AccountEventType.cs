namespace BankingSimulation.Domain.AccountLogs
{
    public enum AccountEventType
    {
        Created = 1,
        Linked = 2,
        Deposit = 3,
        Withdraw = 4,
        Transfer = 5,
        Reassigned = 6,
        Closed = 7
    }
}
