using BankingSimulation.API.Models;
using BankingSimulation.Application.Commands;
using BankingSimulation.Application.Queries;
using BankingSimulation.Domain.AccountHolders;
using BankingSimulation.Domain.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingSimulation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetAccountQuery
            {
                AccountId = id
            }, cancellationToken);

            if (!result.Succeeded)
            {
                Problem();
            }

            return Ok(result.Response);
        }

        [HttpGet("accountHolder/{accountHolderId}")]
        public async Task<IActionResult> GetByAccountHolder(Guid accountHolderId, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetAccountsByAccountHolderQuery
            {
                AccountHolderPublicIdentifier = accountHolderId
            }, cancellationToken);

            if (!result.Succeeded)
            {
                Problem();
            }

            return Ok(result.Response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(NewAccountViewModel account, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new AddAccountCommand
            {
                Account = new Account
                {
                    LinkedAccountId = account.LinkedAccountId,
                    AccountTypeId = account.AccountTypeId,
                    AccountHolder = new AccountHolder
                    {
                        FullName = account.AccountHolderName,
                        PublicIdentifier = account.AccountHolderPublicIdentifier
                    }
                }
            }, cancellationToken);

            if (!result.Succeeded)
            {
                Problem();
            }

            return Ok(result.Response);
        }
    }
}