namespace BankingSimulation.Domain.AccountLogs
{
    public class AccountLog
    {
        public Guid Id { get; set; }

        public AccountEventType EventType { get; set; }

        public Guid AccountId { get; set; }

        public DateTime CreatedDate { get; set; }

        public IDictionary<string, string> Metadata { get; set; }
    }
}
