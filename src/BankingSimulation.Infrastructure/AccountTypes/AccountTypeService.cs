using BankingSimulation.Domain;
using BankingSimulation.Domain.AccountTypes;
using Microsoft.EntityFrameworkCore;

namespace BankingSimulation.Infrastructure.AccountTypes
{
    public class AccountTypeService : IAccountTypeService
    {
        private readonly BankDbContext context;

        public AccountTypeService(BankDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<AccountType>> GetAll()
        {
            return await context.AccountTypes.ToListAsync();
        }
    }
}
