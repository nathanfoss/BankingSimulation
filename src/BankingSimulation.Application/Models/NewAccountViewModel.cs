using BankingSimulation.Domain.AccountTypes;

namespace BankingSimulation.Application.Models
{
    public class NewAccountViewModel
    {
        public string AccountHolderName { get; set; }

        public Guid AccountHolderPublicIdentifier { get; set; }

        public AccountTypeEnum AccountTypeId { get; set; }

        public Guid? LinkedAccountId { get; set; }
    }
}
