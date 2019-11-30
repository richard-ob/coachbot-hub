using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
