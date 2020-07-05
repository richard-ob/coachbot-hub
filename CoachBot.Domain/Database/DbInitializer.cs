using CoachBot.Domain.Model;
using CoachBot.Domain.Models.Dto;
using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoachBot.Database
{
    public static class CoachBotContextExtensions
    {
        public static void Initialize(this CoachBotContext context)
        {
            try
            {
                context.Searches.RemoveRange(context.Searches); // Clear all searches when restarted as timers will have stopped
                context.Lineups.RemoveRange(context.Lineups.Where(t => !context.Matchups.Any(m => m.LineupHomeId == t.Id || m.LineupAwayId == t.Id))); // Remove orphaned teams
                context.PlayerMatchStatistics.RemoveRange(context.PlayerMatchStatistics);
                context.TeamMatchStatistics.RemoveRange(context.TeamMatchStatistics);
                context.Matches.RemoveRange(context.Matches);
                context.MatchStatistics.RemoveRange(context.MatchStatistics);
                context.PlayerTeams.RemoveRange(context.PlayerTeams);
                context.Teams.RemoveRange(context.Teams);
                context.Players.RemoveRange(context.Players);
                context.PlayerPositionMatchStatistics.RemoveRange(context.PlayerPositionMatchStatistics);
                context.PlayerPositions.RemoveRange(context.PlayerPositions);
                context.Positions.RemoveRange(context.Positions);
                context.PlayerLineupPositions.RemoveRange(context.PlayerLineupPositions);
                context.Players.RemoveRange(context.Players);
                //context.PlayerTeamPositions.RemoveRange(context.PlayerTeamPositions.Where(ptp => ptp.CreatedDate < DateTime.Now.AddDays(-1)).Where(ptp => ptp.Team == null || ptp.Team.Match == null || ptp.Team.Match.ReadiedDate == null)); // Clear any signings older than a day
                context.SaveChanges();
            }
            catch
            {
            }
            context.Database.EnsureCreated();
        }

        public static void SeedPreReleaseData(this CoachBotContext context)
        {
            if (context.Regions.Any()) return;

            var regions = new List<Region>()
            {
                new Region()
                {
                    RegionName = "Europe",
                    RegionCode = "EU"
                },
                new Region()
                {
                    RegionName = "South America",
                    RegionCode = "SA"
                }
            };
            context.Regions.AddRange(regions);
            var servers = new List<Server>()
            {
                new Server()
                {
                    Address = "31.33.132.153:27015",
                    Name = "Test Server UK",
                    RegionId = regions.First().RegionId,
                    CountryId = 32
                }
            };
            context.Servers.AddRange(servers);

            var guilds = new List<Guild>()
            {
                new Guild()
                {
                    DiscordGuildId = 310829524277395457,
                    Name = "CoachBot"
                }
            };
            context.Guilds.AddRange(guilds);

            var teams = new List<Team>()
            {
                new Team()
                {
                    GuildId = guilds.First().Id,
                    RegionId = regions.First().RegionId,
                    Name = "Roby",
                    TeamCode = "Roby"
                },
            };
            context.Teams.AddRange(teams);

            var channels = new List<Channel>()
            {
                new Channel()
                {
                    TeamId = teams.First().Id,
                    ChannelType = ChannelType.PrivateMix,
                    UseClassicLineup = true,
                    DisableSearchNotifications = false,
                    Inactive = false,
                    DiscordChannelName = "test",
                    DiscordChannelId = 319213117341040641
                },
                new Channel()
                {
                    TeamId = teams.First().Id,
                    ChannelType = ChannelType.PrivateMix,
                    UseClassicLineup = true,
                    DisableSearchNotifications = false,
                    Inactive = false,
                    DiscordChannelName = "matchmaking",
                    DiscordChannelId = 317415858702123009
                },
                new Channel()
                {
                    TeamId = teams.First().Id,
                    ChannelType = ChannelType.Team,
                    UseClassicLineup = true,
                    DisableSearchNotifications = false,
                    Inactive = false,
                    DiscordChannelName = "Academy",
                    SubTeamName = "Academy",
                    DiscordChannelId = 318123215669166090
                }
            };
            context.Channels.AddRange(channels);

            foreach (var channel in channels)
            {
                channel.ChannelPositions = new List<ChannelPosition>();
                foreach (var position in new string[] { "GK", "LB", "RB", "CF" })
                {
                    channel.ChannelPositions.Add(new ChannelPosition()
                    {
                        ChannelId = channel.Id,
                        Position = new Position()
                        {
                            Name = position
                        }
                    });
                }
            }

            foreach (var position in new string[] { "CB", "LW", "RW", "CM" })
            {
                context.Positions.Add(new Position() { Name = position });
            }

            context.SaveChanges();
        }

        public static void SeedMatchData(this CoachBotContext context)
        {
            context.Matches.RemoveRange(context.Matches);
            context.MatchStatistics.RemoveRange(context.MatchStatistics);
            context.SaveChanges();
            string[] files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2020*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    MatchStatisticsDto matchStatisticsData = JsonConvert.DeserializeObject<MatchStatisticsDto>(json);
                    var match = new Match()
                    {
                        MatchStatistics = new MatchStatistics()
                        {
                            MatchData = matchStatisticsData.MatchData
                        },
                        KickOff = DateTime.UnixEpoch.AddSeconds(matchStatisticsData.MatchData.MatchInfo.StartTime),
                        ServerId = context.Servers.First().Id,
                        TeamHomeId = context.Teams.First().Id,
                        TeamAwayId = context.Teams.First().Id
                    };

                    int currentPlayer = 1;
                    foreach (var player in match.MatchStatistics.MatchData.Players)
                    {
                        var newPlayer = GetOrCreatePlayer(context, player.Info.SteamId64 ?? 0, player.Info.Name);
                        currentPlayer++;
                    }

                    context.Matches.Add(match);
                }
                context.SaveChanges();
            }
            foreach (var player in context.Players)
            {
                var playerTeam = new PlayerTeam()
                {
                    PlayerId = player.Id,
                    TeamId = 1,
                    TeamRole = TeamRole.Player,
                    JoinDate = DateTime.Now.AddMonths(-7)
                };
                context.PlayerTeams.Add(playerTeam);
            }
            context.SaveChanges();
        }

        private static Player GetOrCreatePlayer(CoachBotContext context, ulong steamId, string name)
        {
            var player = context.Players.FirstOrDefault(p => p.SteamID == steamId);

            if (player == null)
            {
                player = new Player()
                {
                    Name = name,
                    SteamID = steamId
                };
                context.Players.Add(player);
            }

            return player;
        }

        public static void SeedTeams(this CoachBotContext context)
        {
            context.Teams.Add(
                new Team()
                {
                    Name = "Ball Breakers",
                    TeamCode = "BB",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.Teams.Add(
                new Team()
                {
                    Name = "Masters of Football",
                    TeamCode = "MOF",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.Teams.Add(
                new Team()
                {
                    Name = "nextGen",
                    TeamCode = "nG",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.Teams.Add(
                new Team()
                {
                    Name = "Phoenix",
                    TeamCode = "phx",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.Teams.Add(
                new Team()
                {
                    Name = "Arrow",
                    TeamCode = "Arrow",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.Teams.Add(
                new Team()
                {
                    Name = "ProSoccer",
                    TeamCode = "PS",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.Teams.Add(
                new Team()
                {
                    Name = "Chefs",
                    TeamCode = "Chefs",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.Teams.Add(
                new Team()
                {
                    Name = "Real Talent",
                    TeamCode = "RT",
                    TeamType = TeamType.Club,
                    RegionId = 2,
                    FoundedDate = DateTime.Now
                }
            );

            context.SaveChanges();
        }
    }
}