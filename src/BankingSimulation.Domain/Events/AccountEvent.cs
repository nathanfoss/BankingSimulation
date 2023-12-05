using BankingSimulation.Domain.Core;

namespace BankingSimulation.Domain.Events
{
    public class AccountEvent : EntityBase<Guid>
    {
        public string Name { get; set; }

        public string Payload { get; set; }
    }
}
