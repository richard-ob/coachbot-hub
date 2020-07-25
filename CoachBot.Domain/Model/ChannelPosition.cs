using CoachBot.Database;
using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class ChannelPosition : IEntity
    {
        public int ChannelId { get; set; }

        public Channel Channel { get; set; }

        public int PositionId { get; set; }

        public Position Position { get; set; }

        public int Ordinal { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }
    }
}