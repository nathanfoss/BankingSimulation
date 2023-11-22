namespace BankingSimulation.Domain.Events
{
    public class EventTypes
    {
        public const string AccountCreated = "Account.Created";
        public const string AccountLinked = "Account.Linked";
        public const string AccountReassigned = "Account.Reassigned";
        public const string AccountClosed = "Account.Closed";
        public const string MoneyDeposited = "Money.Deposited";
        public const string MoneyWithdrawn = "Money.Withdrawn";
        public const string MoneyTransferred = "Money.Transferred";
    }
}
