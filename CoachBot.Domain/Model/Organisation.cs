using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class Organisation
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Acronym { get; set; }

        public int LogoImageId { get; set; }

        public AssetImage LogoImage { get; set; }

        public string BrandColour { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}