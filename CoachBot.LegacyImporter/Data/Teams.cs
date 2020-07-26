using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Data
{
    public static class Teams
    {
        public static List<TeamSeed> TeamSeed = new List<TeamSeed>() {
            new TeamSeed()
            {
                TeamName = "Raw Talent",
                TeamCode = "RT",
                ChannelID = 645678070426107904,
                BadgeUrl = "http://www.iosoccer.co.uk/rt-badge.png",
                BadgeEmote = "<:RT:655473848644403260>",
                Colour = "#2463B0",
                TeamType = Domain.Model.TeamType.Club
            },
            new TeamSeed()
            {
                TeamName = "Natural Talent Academy",
                TeamCode = "NTA",
                ChannelID = 456901950315823104,
                BadgeUrl = "http://www.iosoccer.co.uk/nta-badge.png",
                BadgeEmote = "<:NTA:481488592871358464>",
                Colour = "#5cdb33",
                TeamType = Domain.Model.TeamType.Club
            },
            /*new TeamSeed()
            {
                TeamName = "Dreamsent",
                TeamCode = "Dream",
                ChannelID = 736371554035826739,
                BadgeEmote = "",
                Colour = "#FF00FF",
                TeamType = Domain.Model.TeamType.Club
            },*/
            new TeamSeed()
            {
                TeamName = "IOSoccer Overlap",
                TeamCode = "IOSO",
                ChannelID = 252113301004222465,
                BadgeUrl = "http://www.iosoccer.co.uk/iosoccer-logo.png",
                BadgeEmote = "<:ios:455023721472720896>",
                Colour = "#124364",
                TeamType = Domain.Model.TeamType.Mix
            },
            new TeamSeed()
            {
                TeamName = "IOSoccer Premier",
                TeamCode = "IOSP",
                ChannelID = 519051922058117130,
                BadgeUrl = "http://www.iosoccer.co.uk/iosoccer-logo.png",
                BadgeEmote = "<:ios:455023721472720896>",
                Colour = "#124364",
                TeamType = Domain.Model.TeamType.Mix
            },
            new TeamSeed()
            {
                TeamName = "Poland",
                TeamCode = "PL",
                ChannelID = 721767338336321556,
                BadgeUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c9/Herb_Polski.svg/509px-Herb_Polski.svg.png",
                BadgeEmote = ":flag_pl:",
                Colour = "#fefefe",
                TeamType = Domain.Model.TeamType.National
            },
        };
    }
}
