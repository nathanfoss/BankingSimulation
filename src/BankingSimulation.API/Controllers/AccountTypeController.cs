using BankingSimulation.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingSimulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetAllAccountTypesQuery(), cancellationToken);

            if (!result.Succeeded)
            {
                return Problem();
            }

            return Ok(result.Response);
        }
    }
}
