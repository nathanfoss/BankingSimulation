namespace BankingSimulation.Domain.Core
{
    public abstract class EnumEntityBase<T> where T : Enum
    {
        public T Id { get; set; }

        public string Name { get; set; }
    }
}
