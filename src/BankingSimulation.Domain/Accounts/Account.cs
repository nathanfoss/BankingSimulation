using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.AccountTypes;
using BankingSimulation.Domain.Core;

namespace BankingSimulation.Domain.Accounts
{
    public class Account : EntityBase<Guid>
    {
        public Guid AccountHolderId { get; set; }

        public AccountHolder AccountHolder { get; set; }

        public AccountTypeEnum AccountTypeId { get; set; }

        public AccountType AccountType { get; set; }

        public Guid? LinkedAccountId { get; set; }

        public Account? LinkedAccount { get; set; }

        public decimal Balance { get; set; }

        public override string ToString()
        {
            return $"{AccountTypeId}: {Id.ToString().Substring(Id.ToString().Length - 4)}: ${Balance}";
        }
    }
}
