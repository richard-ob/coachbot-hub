using System.ComponentModel.DataAnnotations;

namespace CoachBot.Model
{
    public class Player
    {
        /*[Key]
        public int Id { get; set; }*/

        public string Name { get; set; }

        public ulong? DiscordUserId { get; set; }

        public string DiscordUserMention { get; set; }

        public Position Position { get; set; }

    }
}
