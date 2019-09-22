using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
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
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));               
                optionsBuilder.UseSqlServer(config.SqlConnectionString);
            }
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
