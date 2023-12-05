using BankingSimulation.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingSimulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountLogController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountLogController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetByAccount(Guid accountId, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetAccountLogsQuery { AccountId = accountId }, cancellationToken);
            if (!result.Succeeded)
            {
                Problem();
            }

            return Ok(result.Response);
        }
    }
}
