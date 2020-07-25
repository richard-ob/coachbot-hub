using CoachBot.Database;
using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class PlayerTeam : IUserUpdateableEntity
    {
        [Key]
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public TeamRole TeamRole { get; set; } = TeamRole.Player;

        public bool IsCurrentTeam => LeaveDate == null;

        public bool IsPending { get; set; } = true;

        public DateTime JoinDate { get; set; }

        public DateTime? LeaveDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}