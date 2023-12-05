namespace BankingSimulation.Domain.Core
{
    public abstract class EntityBase<T> where T : struct
    {
        public T Id { get; set; }
    }
}
