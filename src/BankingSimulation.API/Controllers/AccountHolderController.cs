using BankingSimulation.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingSimulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountHolderController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountHolderController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("identifier/{identifier}")]
        public async Task<IActionResult> GetByIdentifier(Guid identifier, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetAccountHolderByIdentifierQuery { PublicIdentifier = identifier }, cancellationToken);

            if (!result.Succeeded)
            {
                return Problem();
            }

            return Ok(result.Response);
        }
    }
}
