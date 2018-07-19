using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Model
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BotContext(serviceProvider.GetRequiredService<DbContextOptions<BotContext>>()))
            {
                // Look for any movies.
                if (context.Channels.Any())
                {
                    return;   // DB has been seeded
                }

                context.Channels.AddRange(
                     new Channel()
                     {
                         Id = 34234243,
                         ClassicLineup = true,
                         Formation = Formation.ThreeOneThree,
                         Positions = new List<Position>()
                         {
                             new Position() { PositionName = "test" }
                         },
                         Team1 = new Team()
                         {
                             Name = "BB",
                             Color = "blue",
                             IsMix = false,
                             KitEmote = ":bb:",
                             Players = new List<Player>(),
                             Substitutes = null
                         }
                     },
                       new Channel()
                       {
                           Id = 3422224243,
                           ClassicLineup = true,
                           Formation = Formation.ThreeOneThree,
                           Positions = new List<Position>()
                         {
                            new Position() { PositionName = "test" }
                         },
                           Team1 = new Team()
                           {
                               Name = "MOF",
                               Color = "blue",
                               IsMix = false,
                               KitEmote = ":mof:",
                               Players = new List<Player>(),
                               Substitutes = null
                           }
                       }
                );
                context.SaveChanges();
            }
        }
    }
}
