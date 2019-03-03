using System.ComponentModel.DataAnnotations;

namespace CoachBot.Model
{
    public class Guild
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

    }
}
