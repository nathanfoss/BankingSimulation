namespace BankingSimulation.API.Models
{
    public class TransferMoneyViewModel
    {
        public Guid FromAccountId { get; set; }

        public Guid ToAccountId { get; set; }

        public decimal Amount { get; set; }
    }
}
