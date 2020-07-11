using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class SubstitutionRequest
    {
        [Key]
        public string Token { get; set; }

        public int ChannelId { get; set; }

        public Channel Channel { get; set; }

        public int ServerId { get; set; }

        public Server Server { get; set; }

        public ulong DiscordMessageId { get; set; }

        public int? AcceptedById { get; set; }

        public Player AcceptedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? AcceptedDate { get; set; }
    }
}