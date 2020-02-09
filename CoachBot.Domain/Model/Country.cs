using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
