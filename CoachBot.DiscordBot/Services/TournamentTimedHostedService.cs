using CoachBot.Database;
using CoachBot.Domain.Services;
using CoachBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class TimedHostedService : IHostedService, IDisposable
{
    private Timer _timer;
    private IServiceProvider _services;

    public TimedHostedService(IServiceProvider services)
    {
        _services = services;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _services.CreateScope())
        {
            var serverManagementService = scope.ServiceProvider.GetRequiredService<ServerManagementService>();
            var coachBotContext = scope.ServiceProvider.GetRequiredService<CoachBotContext>();
            var discordNotificationService = scope.ServiceProvider.GetRequiredService<DiscordNotificationService>();

            var kickOffTimeBuffer = DateTime.UtcNow.AddMinutes(16).ToString("g");

            var matches = coachBotContext.Matches
                .Where(m => m.KickOff != null && ((DateTime)m.KickOff).ToString("g") == kickOffTimeBuffer && m.ServerId != null && m.TournamentId != null)
                .Include(m => m.TeamHome)
                .Include(m => m.TeamAway)
                .Include(m => m.Map);

            foreach (var match in matches)
            {
                await discordNotificationService.SendAuditChannelMessage($"Setting up tournament match: {match.Id} ({match.TeamHome.Name} vs {match.TeamAway.Name}) [KO: {((DateTime)match.KickOff).ToString("g")}]");
                await serverManagementService.PrepareServerTournament(match.Id);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}