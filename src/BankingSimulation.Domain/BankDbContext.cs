using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.AccountTypes;
using BankingSimulation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BankingSimulation.Domain
{
    public class BankDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Bank");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountType>().HasData(Enum.GetValues<AccountTypeEnum>().Select(id => new AccountType
            {
                Id = id,
                Name = id.ToString()
            }));

            modelBuilder.Entity<AccountEventType>().HasData(Enum.GetValues<AccountEventTypeEnum>().Select(id => new AccountEventType
            {
                Id = id,
                Name = id.ToString()
            }));

            modelBuilder.Entity<AccountLog>(b => b.Ignore(nameof(b.Metadata)));
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountType> AccountTypes { get; set; }

        public DbSet<AccountHolder> AccountHolders { get; set; }

        public DbSet<AccountEvent> AccountEvents { get; set; }

        public DbSet<AccountEventType> AccountEventTypes { get; set; }

        public DbSet<AccountLog> AccountLogs { get; set; }
    }
}
