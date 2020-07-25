using CoachBot.Database;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class Tournament : IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; } = false;

        public int TournamentSeriesId { get; set; }

        public TournamentSeries TournamentSeries { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TournamentType TournamentType { get; set; }

        public TeamType TeamType { get; set; }

        public MatchFormat Format { get; set; } = MatchFormat.EightVsEight;

        public int FantasyPointsLimit { get; set; } = 75;

        public ICollection<TournamentStage> TournamentStages { get; set; }

        public ICollection<TournamentMatchDaySlot> TournamentMatchDays { get; set; }

        public ICollection<TournamentStaff> TournamentStaff { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}