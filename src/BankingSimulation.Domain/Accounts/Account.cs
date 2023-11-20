using BankingSimulation.Domain.AccountHolders;

namespace BankingSimulation.Domain.Accounts
{
    public class Account
    {
        public Guid Id { get; set; }

        public Guid AccountHolderId { get; set; }

        public AccountHolder AccountHolder { get; set; }

        public AccountType AccountType { get; set; }

        public Guid? LinkedAccountId { get; set; }

        public Account? LinkedAccount { get; set; }

        public decimal Balance { get; set; }

        public override string ToString()
        {
            return $"{AccountType}: {Id.ToString().Substring(Id.ToString().Length - 4)}: ${Balance}";
        }
    }
}
