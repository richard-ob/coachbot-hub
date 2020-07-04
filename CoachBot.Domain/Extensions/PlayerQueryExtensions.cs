using CoachBot.Database;
using CoachBot.Model;
using System.Linq;

namespace CoachBot.Domain.Extensions
{
    public static class PlayerQueryExtensions
    {
        public static Player GetPlayerBySteamId(this CoachBotContext coachBotContext, ulong steamUserId)
        {
            return coachBotContext.Players.Single(p => p.SteamID == steamUserId);
        }
    }
}