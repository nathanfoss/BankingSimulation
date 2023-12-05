using BankingSimulation.Domain.Core;

namespace BankingSimulation.Domain.AccountHolders
{
    public class AccountHolder : EntityBase<Guid>
    {
        public string FullName { get; set; }

        public Guid PublicIdentifier { get; set; }
    }
}
