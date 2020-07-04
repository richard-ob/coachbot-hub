using System;

namespace CoachBot.LegacyImporter.Model
{
    public class LegacyPlayer
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ulong? DiscordUserId { get; set; }

        public string DiscordUserMention { get; set; }

        public LegacyPosition Position { get; set; }
    }
}