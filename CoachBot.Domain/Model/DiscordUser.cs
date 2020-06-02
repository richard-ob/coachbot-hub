using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class DiscordUser
    {
        public ulong Id { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

    }
}
