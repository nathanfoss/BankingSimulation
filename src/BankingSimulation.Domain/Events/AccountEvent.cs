namespace BankingSimulation.Domain.Events
{
    public class AccountEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Payload { get; set; }
    }
}
