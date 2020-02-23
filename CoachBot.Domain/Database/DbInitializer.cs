﻿using CoachBot.Domain.Model;
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
                context.Lineups.RemoveRange(context.Lineups.Where(t => !context.Matches.Any(m => m.LineupHomeId == t.Id || m.LineupAwayId == t.Id))); // Remove orphaned teams
                context.TeamStatisticTotals.RemoveRange(context.TeamStatisticTotals);
                context.PlayerStatisticTotals.RemoveRange(context.PlayerStatisticTotals);
                context.StatisticTotals.RemoveRange(context.StatisticTotals);
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
                    CountryId = 1
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
                    IsMixChannel = false,
                    UseClassicLineup = true,
                    DisableSearchNotifications = false,
                    Inactive = false,
                    DiscordChannelName = "test",
                    DiscordChannelId = 319213117341040641
                },
                new Channel()
                {
                    TeamId = teams.First().Id,
                    IsMixChannel = true,
                    UseClassicLineup = true,
                    DisableSearchNotifications = false,
                    Inactive = false,
                    DiscordChannelName = "matchmaking",
                    DiscordChannelId = 317415858702123009
                },
                new Channel()
                {
                    TeamId = teams.First().Id,
                    IsMixChannel = false,
                    UseClassicLineup = true,
                    DisableSearchNotifications = false,
                    Inactive = false,
                    DiscordChannelName = "Academy",
                    SubTeamName = "Academy",
                    DiscordChannelId = 318123215669166090
                }
            };
            context.Channels.AddRange(channels);

            foreach(var channel in channels)
            {
                channel.ChannelPositions = new List<ChannelPosition>();
                foreach(var position in new string[] { "GK", "LB", "RB", "CF" })
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

            context.SaveChanges();
        }

        public static void SeedMatchData(this CoachBotContext context)
        {
            context.Matches.RemoveRange(context.Matches);
            context.MatchStatistics.RemoveRange(context.MatchStatistics);
            context.SaveChanges();
            string[] files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2020*.json", SearchOption.AllDirectories);
            foreach(var file in files)
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
                        ReadiedDate = DateTime.UnixEpoch.AddSeconds(matchStatisticsData.MatchData.MatchInfo.StartTime),
                        ServerId = context.Servers.First().Id,
                        LineupHome = new Lineup()
                        {
                            ChannelId = context.Channels.First().Id,
                            TeamType = TeamType.Home
                        },
                        LineupAway = new Lineup()
                        {
                            ChannelId = context.Channels.First(c => c.Id != context.Channels.First().Id).Id,
                            TeamType = TeamType.Away                            
                        }
                    };

                    context.Matches.Add(match);
                }
            }
            context.SaveChanges();
        }
    }
}
