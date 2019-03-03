using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Model
{
    public class User
    {
        [Key]
        public ulong DiscordUserId { get; set; }

        public string Name { get; set; }

        public string DiscordUserIdString { get { return DiscordUserId.ToString(); } }

        public bool IsAdministrator { get; set; }

        public IEnumerable<Channel> Channels { get; set; }

    }
}
