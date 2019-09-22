using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Region
    {
        [Key]
        public int RegionId { get; set; }

        public string RegionName { get; set; }

        [JsonIgnore]
        public int ServerCount { get; set; }
    }
}
