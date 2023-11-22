using BankingSimulation.Application.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BankingSimulation.Infrastructure.Events
{
    public class AccountEventBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        private TimeSpan period = TimeSpan.FromSeconds(3);

        public AccountEventBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            using var timer = new PeriodicTimer(period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await mediator.Send(new AddAccountLogCommand(), stoppingToken);
            }
        }
    }
}
