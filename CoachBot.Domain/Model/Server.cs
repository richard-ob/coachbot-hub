using System.ComponentModel.DataAnnotations;

namespace CoachBot.Model
{
    public class Server
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string RconPassword { get; set; }

        public int RegionId { get; set; }

        public Region Region { get; set; }

    }
}
