using CoachBot.Domain.Model;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Data
{
    public static class Captains
    {
        public static List<Captain> CaptainsList => new List<Captain>()
        {
            new Captain()
            {
                TeamName = "Excel",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Bas",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198088510993
                }
            },
            new Captain()
            {
                TeamName = "False 11",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Josh",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561197964312016
                }
            },
            new Captain()
            {
                TeamName = "False 11",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Tet",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561197962470520
                }
            },
            new Captain()
            {
                TeamName = "Natural Talent",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Wilmots",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561197960902023
                }
            },
            new Captain()
            {
                TeamName = "NextGen",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Aryan",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198011665624
                }
            },
            new Captain()
            {
                TeamName = "Revolution",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Nuri",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198000232781
                }
            },
            new Captain()
            {
                TeamName = "Revolution",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Janir",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198170755321
                }
            },
            new Captain()
            {
                TeamName = "Tempest",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Kobe",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198000869062
                }
            },
            new Captain()
            {
                TeamName = "Tempest",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "NightFire",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561197978960120
                }
            },
            /*new Captain()
            {
                TeamName = "Unity",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Kieran",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198061658353
                }
            },*/
            new Captain()
            {
                TeamName = "Dreamsent",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Phenom",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198214708181
                }
            },
            new Captain()
            {
                TeamName = "xGoal",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Kaim",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198208126212
                }
            },
            new Captain()
            {
                TeamName = "xGoal",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Sir God Rock",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198001762940
                }
            },
            new Captain()
            {
                TeamName = "Cryptic",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Valor",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198397197697
                }
            },
            new Captain()
            {
                TeamName = "Cryptic",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "KayZ",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198974493259
                }
            },
            new Captain()
            {
                TeamName = "CABRONES",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "ericFM",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198055048134
                }
            },
            new Captain()
            {
                TeamName = "CABRONES",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "jota",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561197999990696
                }
            },
            new Captain()
            {
                TeamName = "Natural Talent Academy",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Kawhi Leonard",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198962410642
                }
            },
            new Captain()
            {
                TeamName = "Raw Talent",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Jiggy",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561197960577686
                }
            },
            new Captain()
            {
                TeamName = "Raw Talent",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Sandwich",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198313910129
                }
            },
            new Captain()
            {
                TeamName = "Tiger Haxball Club",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "Rubiger",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198800638741
                }
            },
            new Captain()
            {
                TeamName = "Tiger Haxball Club",
                Role = TeamRole.Captain,
                Player = new Player()
                {
                    Name = "shaq",
                    HubRole = PlayerHubRole.Player,
                    SteamID =  76561198051949901
                }
            },
        };
    }

    public struct Captain
    {
        public Player Player { get; set; }

        public string TeamName { get; set; }

        public TeamRole Role { get; set; }

    }
}
