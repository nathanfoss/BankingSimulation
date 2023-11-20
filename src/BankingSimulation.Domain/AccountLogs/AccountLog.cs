namespace BankingSimulation.Domain.AccountLogs
{
    public class AccountLog
    {
        public Guid Id { get; set; }

        public AccountEventType EventType { get; set; }

        public Guid AccountId { get; set; }

        public DateTime CreatedDate { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public override string ToString()
        {
            var metadata = string.Empty;
            foreach (var data in Metadata)
            {
                metadata += $"{data.Key}: {data.Value} \r\n";
            }
            return $"Account {AccountId}: {EventType}: at {CreatedDate.ToString("f")} \r\n {metadata}";
        }
    }
}
