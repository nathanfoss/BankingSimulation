namespace BankingSimulation.Domain.AccountHolders
{
    public class AccountHolder
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public Guid PublicIdentifier { get; set; }
    }
}
