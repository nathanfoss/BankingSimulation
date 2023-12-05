using BankingSimulation.API.Models;
using BankingSimulation.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingSimulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyController : ControllerBase
    {
        private readonly IMediator mediator;

        public MoneyController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut("deposit")]
        public async Task<IActionResult> DepositMoney(TransactionViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new DepositMoneyCommand
            {
                AccountId = viewModel.AccountId,
                Amount = viewModel.Amount
            }, cancellationToken);

            if (!result.Succeeded)
            {
                Problem();
            }

            return Accepted();
        }

        [HttpPut("withdraw")]
        public async Task<IActionResult> WithdrawMoney(TransactionViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new WithdrawMoneyCommand
            {
                AccountId = viewModel.AccountId,
                Amount = viewModel.Amount
            }, cancellationToken);

            if (!result.Succeeded)
            {
                Problem();
            }

            return Accepted();
        }

        [HttpPut("transfer")]
        public async Task<IActionResult> WithdrawMoney(TransferMoneyViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new TransferMoneyCommand
            {
                FromAccountId = viewModel.FromAccountId,
                ToAccountId = viewModel.ToAccountId,
                Amount = viewModel.Amount
            }, cancellationToken);

            if (!result.Succeeded)
            {
                Problem();
            }

            return Accepted();
        }
    }
}
