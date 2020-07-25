using CoachBot.Database;
using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class Organisation : IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Acronym { get; set; }

        public int LogoImageId { get; set; }

        public AssetImage LogoImage { get; set; }

        public string BrandColour { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}