using System.Collections.Generic;

namespace CoachBot.Domain.Model
{
    public class DiscordGuild
    {
        public ulong Id { get; set; }

        public string Name { get; set; }

        public bool IsLinked { get; set; }

        public List<KeyValuePair<string, string>> Emotes { get; set; }
    }
}