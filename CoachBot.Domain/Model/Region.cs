using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        public string RegionName { get; set; }

        public int ServerCount { get; set; }
    }
}
