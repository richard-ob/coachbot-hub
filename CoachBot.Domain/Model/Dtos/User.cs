using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    [NotMapped]
    public class User
    {
        public ulong SteamId { get; set; }

        public string Name { get; set; }

        public string SteamIdString { get { return SteamId.ToString(); } }

        public bool IsAdministrator { get; set; }

        public int PlayerId { get; set; }
    }
}