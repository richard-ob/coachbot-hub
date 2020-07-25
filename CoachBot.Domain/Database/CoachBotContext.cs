using CoachBot.Domain.Database;
using CoachBot.Domain.Model;
using CoachBot.Model;
using CoachBot.Shared.Extensions;
using CoachBot.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoachBot.Database
{
    public class CoachBotContext : DbContext
    {
        public CoachBotContext(DbContextOptions options)
               : base(options)
        {
            ChangeTracker.Tracked += OnEntityTracked;
            ChangeTracker.StateChanged += OnEntityStateChanged;
        }

        public DbSet<Server> Servers { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerTeam> PlayerTeams { get; set; }
        public DbSet<PlayerPosition> PlayerPositions { get; set; }
        public DbSet<PlayerLineupPosition> PlayerLineupPositions { get; set; }
        public DbSet<PlayerLineupSubstitute> PlayerLineupSubstitutes { get; set; }
        public DbSet<ChannelPosition> ChannelPositions { get; set; }
        public DbSet<Lineup> Lineups { get; set; }
        public DbSet<Matchup> Matchups { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<SubstitutionRequest> SubstitutionRequests { get; set; }
        public DbSet<Search> Searches { get; set; }
        public DbSet<MatchStatistics> MatchStatistics { get; set; }
        public DbSet<TeamMatchStatistics> TeamMatchStatistics { get; set; }
        public DbQuery<TeamPerformanceSnapshot> TeamPerformanceSnapshots { get; set; }
        public DbSet<PlayerPositionMatchStatistics> PlayerPositionMatchStatistics { get; set; }
        public DbSet<PlayerMatchStatistics> PlayerMatchStatistics { get; set; }
        public DbSet<PlayerRating> PlayerRatings { get; set; }
        public DbQuery<PlayerPerformanceSnapshot> PlayerPerformanceSnapshots { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<TournamentSeries> TournamentSeries { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentStage> TournamentStages { get; set; }
        public DbSet<TournamentPhase> TournamentPhases { get; set; }
        public DbSet<TournamentGroup> TournamentGroups { get; set; }
        public DbSet<TournamentGroupMatch> TournamentGroupMatches { get; set; }
        public DbSet<TournamentGroupTeam> TournamentGroupTeams { get; set; }
        public DbSet<TournamentMatchDaySlot> TournamentMatchDays { get; set; }
        public DbSet<TournamentStaff> TournamentStaff { get; set; }
        public DbSet<FantasyTeam> FantasyTeams { get; set; }
        public DbSet<FantasyPlayer> FantasyPlayers { get; set; }
        public DbSet<FantasyTeamSelection> FantasyTeamSelections { get; set; }
        public DbSet<FantasyPlayerPhase> FantasyPlayerPhases { get; set; }
        public DbQuery<FantasyTeamRank> FantasyTeamRanks { get; set; }
        public DbQuery<FantasyPlayerRank> FantasyPlayerRanks { get; set; }
        public DbSet<ScorePrediction> ScorePredictions { get; set; }
        public DbSet<AssetImage> AssetImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = ConfigHelper.GetConfig();
                optionsBuilder.UseSqlServer(config.SqlConnectionString, o => { o.EnableRetryOnFailure(); });
            }
        }

        private void OnEntityStateChanged(object sender, EntityStateChangedEventArgs e)
        {
            if (e.NewState == EntityState.Modified)
            {
                if (e.Entry.Entity is IUserUpdateableEntity userUpdateableEntity)
                {
                    userUpdateableEntity.UpdatedDate = DateTime.UtcNow;
                    var playerId = CallContext.GetData(CallContextDataType.PlayerId);
                    if (playerId != null)
                    {
                        userUpdateableEntity.UpdatedById = (int?)playerId;
                    }
                }
                else if (e.Entry.Entity is IUpdateableEntity updateableEntity)
                {
                    updateableEntity.UpdatedDate = DateTime.UtcNow;
                }
                else if (e.Entry.Entity is ISystemUpdateableEntity systemUpdateableEntity)
                {
                    systemUpdateableEntity.UpdatedDate = DateTime.UtcNow;
                }
            }
        }

        void OnEntityTracked(object sender, EntityTrackedEventArgs e)
        {
            if (!e.FromQuery && e.Entry.State == EntityState.Added)
            {
                if (e.Entry.Entity is IUserUpdateableEntity userUpdateableEntity)
                {
                    userUpdateableEntity.CreatedDate = DateTime.UtcNow;
                    userUpdateableEntity.UpdatedDate = DateTime.UtcNow;
                    var playerId = CallContext.GetData(CallContextDataType.PlayerId);
                    if (playerId != null)
                    {
                        userUpdateableEntity.CreatedById = (int?)playerId;
                        userUpdateableEntity.UpdatedById = (int?)playerId;
                    }
                }
                else if (e.Entry.Entity is IUpdateableEntity updateableEntity)
                {
                    updateableEntity.CreatedDate = DateTime.UtcNow;
                    updateableEntity.UpdatedDate = DateTime.UtcNow;
                    var playerId = CallContext.GetData(CallContextDataType.PlayerId);
                    if (playerId != null)
                    {
                        updateableEntity.CreatedById = (int?)playerId;
                    }
                }
                else if (e.Entry.Entity is ISystemEntity systemEntity)
                {
                    systemEntity.CreatedDate = DateTime.UtcNow;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many composite primary keys
            modelBuilder.Entity<PlayerPosition>().HasKey(pp => new { pp.PlayerId, pp.PositionId });
            modelBuilder.Entity<PlayerLineupPosition>().HasKey(ptp => new { ptp.PlayerId, ptp.PositionId, ptp.LineupId });
            modelBuilder.Entity<PlayerLineupSubstitute>().HasKey(ptp => new { ptp.PlayerId, ptp.LineupId });
            modelBuilder.Entity<ChannelPosition>().HasKey(cp => new { cp.ChannelId, cp.PositionId });

            // Manual relationships
            modelBuilder.Entity<Player>().HasMany(p => p.Teams).WithOne("Player");
            modelBuilder.Entity<Player>().HasMany(p => p.CreatedPlayerTeams).WithOne("CreatedBy");
            modelBuilder.Entity<Player>().HasMany(p => p.UpdatedPlayerTeams).WithOne("UpdatedBy");
            modelBuilder.Entity<Player>().HasMany(p => p.CreatedPlayers).WithOne("CreatedBy");
            modelBuilder.Entity<Player>().HasMany(p => p.UpdatedPlayers).WithOne("UpdatedBy");

            // Unique constraints
            modelBuilder.Entity<Channel>().HasIndex(c => new { c.DiscordChannelId }).IsUnique(true);
            modelBuilder.Entity<Guild>().HasIndex(g => new { g.DiscordGuildId }).IsUnique(true);
            modelBuilder.Entity<Server>().HasIndex(s => new { s.Address }).IsUnique(true);
            modelBuilder.Entity<Region>().HasIndex(r => new { r.RegionName }).IsUnique(true);
            modelBuilder.Entity<Team>().HasIndex(t => new { t.TeamCode, t.RegionId }).IsUnique(true);
            modelBuilder.Entity<Position>().HasIndex(p => new { p.Name }).IsUnique(true);
            modelBuilder.Entity<PlayerLineupPosition>().HasIndex(ptp => new { ptp.PositionId, ptp.LineupId }).IsUnique(true);
            modelBuilder.Entity<TournamentStaff>().HasIndex(tes => new { tes.PlayerId, tes.TournamentId }).IsUnique(true);
            modelBuilder.Entity<FantasyTeam>().HasIndex(ft => new { ft.PlayerId, ft.TournamentId }).IsUnique(true);

            // Defaults
            modelBuilder.Entity<Channel>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Team>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Match>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Server>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Region>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Player>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Lineup>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Matchup>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Country>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Guild>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Position>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<MatchStatistics>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TeamMatchStatistics>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<PlayerMatchStatistics>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<PlayerPositionMatchStatistics>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<PlayerPosition>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<PlayerLineupPosition>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<PlayerLineupSubstitute>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<PlayerTeam>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ChannelPosition>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentSeries>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Tournament>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentStage>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentPhase>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentGroup>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentGroupMatch>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentGroupTeam>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentMatchDaySlot>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentSeries>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TournamentStaff>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Organisation>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FantasyPlayer>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FantasyTeam>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FantasyPlayerPhase>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FantasyTeamSelection>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ScorePrediction>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<AssetImage>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");

            // Conversions
            modelBuilder.Entity<Search>().Property(p => p.DiscordSearchMessages).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(v));
            modelBuilder.Entity<MatchStatistics>().Property(p => p.MatchData).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<MatchData>(v));
            modelBuilder.Entity<Channel>().Property(p => p.SearchIgnoreList).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<int>>(v));
            modelBuilder.Entity<Team>().Property(p => p.Form).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<MatchOutcomeType>>(v));

            // Seed data
            modelBuilder.Entity<Country>().HasData(CountrySeedData.GetCountries());
        }
    }

    internal interface ISystemEntity
    {
        DateTime CreatedDate { get; set; }
    }

    internal interface ISystemUpdateableEntity : ISystemEntity
    {
        DateTime UpdatedDate { get; set; }
    }

    internal interface IEntity: ISystemEntity
    {
        int? CreatedById { get; set; }
        Player CreatedBy { get; set; }
    }

    internal interface IUpdateableEntity: IEntity
    {
        DateTime UpdatedDate { get; set; }
    }

    internal interface IUserUpdateableEntity: IUpdateableEntity
    {        
        int? UpdatedById { get; set; }
        Player UpdatedBy { get; set; }
    }

    public class CoachBotContextFactory : IDesignTimeDbContextFactory<CoachBotContext>
    {
        public CoachBotContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CoachBotContext>();
            var config = ConfigHelper.GetConfig();
            builder.UseSqlServer(config.SqlConnectionString, o => { o.EnableRetryOnFailure(); });
            return new CoachBotContext(builder.Options);
        }
    }
}