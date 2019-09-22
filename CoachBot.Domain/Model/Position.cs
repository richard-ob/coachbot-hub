using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Position
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Position() {}

        public Position(string position)
        {
            PositionName = position;
        }

        public string PositionName { get; set; }

        public ulong ChannelId { get; set; }

        public Channel Channel { get; set; }
    }
}
