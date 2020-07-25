using CoachBot.Database;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TournamentSeries : IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public bool IsActive { get; set; }

        public int? TournamentLogoId { get; set; }

        public AssetImage TournamentLogo { get; set; }

        public int OrganisationId { get; set; }

        public Organisation Organisation { get; set; }

        public ICollection<Tournament> Tournaments { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}