using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class Search
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ChannelId { get; set; }

        public Channel Channel { get; set; }

        public Dictionary<ulong, ulong> DiscordSearchMessages { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
