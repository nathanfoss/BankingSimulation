namespace BankingSimulation.Domain.AccountTypes
{
    public interface IAccountTypeService
    {
        Task<IEnumerable<AccountType>> GetAll();
    }
}
