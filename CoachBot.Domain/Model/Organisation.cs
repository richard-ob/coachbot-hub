using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class Organisation
    {
        [Key]
        public int Id { get; set; }

        public string OrganisationName { get; set; }

    }
}