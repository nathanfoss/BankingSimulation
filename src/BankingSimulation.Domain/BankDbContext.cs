using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.AccountLogs;
using BankingSimulation.Domain.Accounts;
using BankingSimulation.Domain.AccountTypes;
using BankingSimulation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            modelBuilder.Entity<AccountLog>()
            .Property(b => b.Metadata)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountType> AccountTypes { get; set; }

        public DbSet<AccountHolder> AccountHolders { get; set; }

        public DbSet<AccountEvent> AccountEvents { get; set; }

        public DbSet<AccountEventType> AccountEventTypes { get; set; }

        public DbSet<AccountLog> AccountLogs { get; set; }
    }
}
