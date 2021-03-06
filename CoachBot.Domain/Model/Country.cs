﻿using CoachBot.Database;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class Country: ISystemEntity
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string DiscordFlagEmote => $":flag_{Code.ToLower()}:";

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}