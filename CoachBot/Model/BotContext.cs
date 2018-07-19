using Microsoft.EntityFrameworkCore;

namespace CoachBot.Model
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions<BotContext> options)
            : base(options)
        { }

        public DbSet<Channel> Channels { get; set; }
    }
}
