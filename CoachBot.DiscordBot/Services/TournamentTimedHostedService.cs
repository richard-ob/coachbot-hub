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

            // INFO: Restart servers 20 minutes before game
            var kickOffTimeRestartBuffer = DateTime.UtcNow.AddMinutes(20).ToString("g");
            var restartMatches = coachBotContext.Matches
                .AsQueryable()
                .Where(m => m.KickOff > DateTime.UtcNow)
                .Include(m => m.TeamHome)
                .Include(m => m.TeamAway)
                .Include(m => m.Map)
                .ToList()
                .Where(m => m.KickOff != null && ((DateTime)m.KickOff).ToString("g") == kickOffTimeRestartBuffer && m.ServerId != null && m.TournamentId != null);

            foreach (var match in restartMatches)
            {
                await discordNotificationService.SendAuditChannelMessage($"Restarting server for tournament match: {match.Id} ({match.TeamHome.Name} vs {match.TeamAway.Name}) [KO: {((DateTime)match.KickOff).ToString("g")}]");
                await serverManagementService.RestartServer(match.ServerId.Value);
            }

            // INFO: Setup servers ~15 minutes for the game
            var kickOffTimeSetupBuffer = DateTime.UtcNow.AddMinutes(16).ToString("g");
            var kickOffMatches = coachBotContext.Matches
                .AsQueryable()
                .Where(m => m.KickOff > DateTime.UtcNow)
                .Include(m => m.TeamHome)
                .Include(m => m.TeamAway)
                .Include(m => m.Map)
                .ToList()
                .Where(m => m.KickOff != null && ((DateTime)m.KickOff).ToString("g") == kickOffTimeSetupBuffer && m.ServerId != null && m.TournamentId != null);

            foreach (var match in kickOffMatches)
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