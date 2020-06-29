using CoachBot.Domain.Model;
using CoachBot.LegacyImporter.Model;
using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoachBot.LegacyImporter
{
    public class Importer
    {
        private readonly DiscordSocketClient discordSocketClient;
        public readonly LegacyConfig config;

        public List<Region> Regions;
        public List<Server> Servers;
        public List<Position> Positions;

        public Importer(DiscordSocketClient discordSocketClient)
        {
            config = JsonConvert.DeserializeObject<LegacyConfig>(File.ReadAllText(@"legacy-config.json"));
            this.discordSocketClient = discordSocketClient;
            this.Regions = GetRegions();
            this.Servers = GetServers();
            this.Positions = GetPositions();
        }

        public List<Region> GetRegions()
        {
            var regions = new List<Region>();

            foreach (var legacyRegion in config.Regions.OrderBy(r => r.RegionId))
            {
                var region = new Region()
                {
                    RegionName = legacyRegion.RegionName,
                    RegionCode = legacyRegion.RegionName.Substring(0, 3).ToUpper()
                };

                regions.Add(region);
            }

            return regions;
        }

        public List<Server> GetServers()
        {
            var servers = new List<Server>();

            foreach (var legacyServer in config.Servers)
            {
                var server = new Server()
                {
                    Name = legacyServer.Name,
                    RegionId = legacyServer.RegionId,
                    Address = legacyServer.Address,
                    RconPassword = legacyServer.RconPassword,
                    IsActive = true
                };

                servers.Add(server);
            }

            return servers;
        }

        public List<Position> GetPositions()
        {
            var positions = new List<Position>();

            foreach (var channel in config.Channels)
            {
                var newPositions = channel.Positions.Select(p => new Position()
                {
                    Name = p.PositionName
                }).Where(p => !positions.Any(e => e.Name.ToUpper() == p.Name.ToUpper()));

                positions.AddRange(newPositions);
            }

            return positions;
        }

        public List<Team> GetTeams()
        {
            var guilds = new List<Guild>();
            var teams = new List<Team>();
            var channels = new List<Channel>();

            var currentGuildId = 1;
            foreach(var guildChannel in config.Channels.GroupBy(c => c.GuildName).Select(s => new { GuildName = s.Key, ChannelID = s.Max(p => p.Id) }))
            {
                var discordChannel = this.discordSocketClient.GetChannel(guildChannel.ChannelID) as ITextChannel;

                try
                {
                    if (!string.IsNullOrEmpty(discordChannel.Guild.IconUrl))
                    {
                        var assetImage = HttpImageRetrieval.GetImageAsBase64(discordChannel.Guild.IconUrl);
                        // Save into storage
                    }

                    var guild = new Guild()
                    {
                        Id = currentGuildId,
                        DiscordGuildId = discordChannel.GuildId,
                        Name = guildChannel.GuildName
                    };
                    guilds.Add(guild);
                }
                catch
                {

                }
                currentGuildId++;
            }

            foreach(var guild in guilds)
            {
                // MAYBE IMPORT GUILD IMAGES :D
                try
                {
                    var leadChannel = config.Channels
                    .OrderBy(c => c.LastSearch.HasValue)
                    .OrderBy(c => c.LastSearch)
                    .OrderBy(c => !string.IsNullOrWhiteSpace(c.Team1.BadgeEmote))
                    .First(c => c.GuildName == guild.Name);

                    var team = new Team()
                    {
                        TeamCode = guild.Name,
                        Name = guild.Name,
                        BadgeEmote = leadChannel.Team1.BadgeEmote,
                        KitEmote = leadChannel.Team1.KitEmote,
                        RegionId = leadChannel.RegionId,
                        TeamType = TeamType.Club,
                        Color = leadChannel.Team1.Color,
                        GuildId = guild.Id
                    };

                    teams.Add(team);
                }
                catch { }
            }

            foreach(var legacyChannel in config.Channels)
            {
                var discordChannel = this.discordSocketClient.GetChannel(legacyChannel.Id) as ITextChannel;
                var currentPosIndex = 0;
                try
                {
                    var channel = new Channel()
                    {
                        TeamId = teams.First(t => t.GuildId == guilds.First(g => g.Name == legacyChannel.GuildName).Id).Id,
                        DiscordChannelId = legacyChannel.Id,
                        DuplicityProtection = legacyChannel.EnableUnsignWhenPlayerStartsOtherGame,
                        IsMixChannel = legacyChannel.IsMixChannel,
                        DisableSearchNotifications = legacyChannel.DisableSearchNotifications,
                        UseClassicLineup = legacyChannel.ClassicLineup,
                        DiscordChannelName = legacyChannel.Name,
                        Formation = (Formation)legacyChannel.Formation,                        
                        ChannelPositions = this.Positions.Where(p => legacyChannel.Positions.Any(l => l.PositionName.ToUpper() == p.Name.ToUpper())).Select(p =>
                            new ChannelPosition()
                            {
                                PositionId = p.Id,
                                Ordinal = currentPosIndex++
                            }) as ICollection<ChannelPosition> 
                    };

                    channels.Add(channel);
                }
                catch
                {

                }
            }

            return teams;
        }

    }
}
