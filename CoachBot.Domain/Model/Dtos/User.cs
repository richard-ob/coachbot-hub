using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    [NotMapped]
    public class User
    {
        public ulong DiscordUserId { get; set; }

        public string Name { get; set; }

        public string DiscordUserIdString { get { return DiscordUserId.ToString(); } }

        public bool IsAdministrator { get; set; }

    }
}
