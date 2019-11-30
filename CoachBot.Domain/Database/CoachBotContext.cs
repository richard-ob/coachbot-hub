using CoachBot.Domain.Model;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CoachBot.Database
{
    public class CoachBotContext : DbContext
    {
        public CoachBotContext(DbContextOptions options)
               : base(options)
        { }
        
        public DbSet<Server> Servers { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerTeamPosition> PlayerTeamPositions { get; set; }
        public DbSet<PlayerTeamSubstitute> PlayerTeamSubstitute { get; set; }
        public DbSet<ChannelPosition> ChannelPositions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<SubstitutionRequest> SubstitutionRequests { get; set; }
        public DbSet<Search> Searches { get; set; }
        public DbSet<MatchStatistics> MatchStatistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
                optionsBuilder.UseSqlServer(config.SqlConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many composite primary keys
            modelBuilder.Entity<PlayerTeamPosition>().HasKey(ptp => new { ptp.PlayerId, ptp.PositionId, ptp.TeamId });
            modelBuilder.Entity<PlayerTeamSubstitute>().HasKey(ptp => new { ptp.PlayerId, ptp.TeamId });
            modelBuilder.Entity<ChannelPosition>().HasKey(cp => new { cp.ChannelId, cp.PositionId });

            // Unique constraints
            modelBuilder.Entity<Channel>().HasIndex(c => new { c.DiscordChannelId }).IsUnique(true);
            modelBuilder.Entity<Server>().HasIndex(s => new { s.Address }).IsUnique(true);
            modelBuilder.Entity<Region>().HasIndex(r => new { r.RegionName }).IsUnique(true);
            modelBuilder.Entity<MatchStatistics>().HasIndex(ms => new { ms.MatchId }).IsUnique(true);
            modelBuilder.Entity<PlayerTeamPosition>().HasIndex(ptp => new { ptp.PositionId, ptp.TeamId }).IsUnique();

            // Conversions
            modelBuilder.Entity<Search>().Property(p => p.DiscordMessageIds).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<ulong>>(v));
            modelBuilder.Entity<MatchStatistics>().Property(p => p.MatchData).HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<MatchData>(v));
        }
    }

    public class CoachBotContextFactory : IDesignTimeDbContextFactory<CoachBotContext>
    {

        public CoachBotContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CoachBotContext>();
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            builder.UseSqlServer(config.SqlConnectionString);
            return new CoachBotContext(builder.Options);
        }
    }
}
