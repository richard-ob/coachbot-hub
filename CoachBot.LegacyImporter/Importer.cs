using CoachBot.Database;
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
        private readonly CoachBotContext coachBotContext;
        public readonly LegacyConfig config;
        public readonly List<LegacyMatch> matchHistory;

        public List<Region> Regions;
        public List<Server> Servers;
        public List<Position> Positions;
        public Dictionary<string, AssetImage> TeamAssetImages = new Dictionary<string, AssetImage>();

        public Importer(DiscordSocketClient discordSocketClient, CoachBotContext coachBotContext)
        {
            matchHistory = JsonConvert.DeserializeObject<List<LegacyMatch>>(File.ReadAllText(@"history.json"));
            config = JsonConvert.DeserializeObject<LegacyConfig>(File.ReadAllText(@"legacy-config.json"));
            this.discordSocketClient = discordSocketClient;
            this.coachBotContext = coachBotContext;
            this.Regions = GetRegions();
            this.Servers = GetServers();
            this.Positions = GetPositions();
            SetupAdministrator();
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

            this.coachBotContext.Regions.AddRange(regions);
            this.coachBotContext.SaveChanges();
            return regions;
        }

        public List<Server> GetServers()
        {
            var servers = new List<Server>();

            foreach (var legacyServer in config.Servers)
            {
                var countryCode = IpTools.GetCountryFromIpData(legacyServer.Address.Split(":")[0]);
                var countryId = this.coachBotContext.Countries.Where(c => c.Code == countryCode).Select(c => c.Id).FirstOrDefault();
                var server = new Server()
                {
                    Name = legacyServer.Name,
                    RegionId = legacyServer.RegionId,
                    Address = legacyServer.Address,
                    RconPassword = legacyServer.RconPassword,
                    IsActive = true,
                    CountryId = countryId > 0 ? countryId : 1
                };

                servers.Add(server);
            }

            this.coachBotContext.Servers.AddRange(servers);
            this.coachBotContext.SaveChanges();
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

            this.coachBotContext.Positions.AddRange(positions);
            this.coachBotContext.SaveChanges();
            return positions;
        }

        public List<Team> GetTeams()
        {
            var guilds = new List<Guild>();
            var teams = new List<Team>();
            var channels = new List<Channel>();

            var activeChannels = config.Channels
                .GroupBy(c => c.GuildName)
                .Select(s => new
                {
                    GuildName = s.Key,
                    ChannelID = s.Max(p => p.Id),
                    HasMatchCount = s.Count(p => matchHistory.Any(m => m.ChannelId == p.Id && m.MatchDate > DateTime.Now.AddMonths(-3)))
                })
                .Where(t => t.HasMatchCount > 0);

            foreach (var guildChannel in activeChannels)
            {
                var discordChannel = this.discordSocketClient.GetChannel(guildChannel.ChannelID) as ITextChannel;

                try
                {
                    if (!string.IsNullOrEmpty(discordChannel.Guild.IconUrl))
                    {
                        var assetImageContent = HttpImageRetrieval.GetImageAsBase64(discordChannel.Guild.IconUrl.Replace(".jpg", ".png") + "?size=512");
                        var assetImage = new AssetImage()
                        {
                            Base64EncodedImage = assetImageContent,
                            FileName = discordChannel.Name.Replace("#", "").Replace(" ", "-"),
                            PlayerId = 1
                        };
                        this.coachBotContext.AssetImages.Add(assetImage);
                        this.TeamAssetImages.Add(guildChannel.GuildName, assetImage);
                    }

                    var guild = new Guild()
                    {
                        DiscordGuildId = discordChannel.GuildId,
                        Name = guildChannel.GuildName
                    };
                    guilds.Add(guild);
                    this.coachBotContext.Guilds.Add(guild);
                }
                catch
                {

                }
            }
            this.coachBotContext.SaveChanges();

            foreach (var guild in guilds)
            {
                // MAYBE IMPORT GUILD IMAGES :D
                try
                {
                    var leadChannel = config.Channels
                        .Where(c => c.RegionId > 0)
                        .OrderBy(c => c.LastSearch.HasValue)
                        .ThenBy(c => c.LastSearch)
                        .ThenBy(c => !string.IsNullOrWhiteSpace(c.Team1.BadgeEmote))
                        .First(c => c.GuildName == guild.Name);

                    if (leadChannel == null)
                    {
                        continue;
                    }

                    var team = new Team()
                    {
                        TeamCode = guild.Name,
                        Name = guild.Name,
                        BadgeEmote = leadChannel.Team1.BadgeEmote,
                        KitEmote = leadChannel.Team1.KitEmote,
                        RegionId = leadChannel.RegionId,
                        TeamType = TeamTypeMapper.GetTeamTypeForTeam(guild.Name),
                        Color = leadChannel.Team1.Color,
                        GuildId = guild.Id,
                        BadgeImageId = this.TeamAssetImages.Where(t => t.Key == guild.Name).Select(t => (int?)t.Value.Id).FirstOrDefault()
                    };

                    teams.Add(team);
                    this.coachBotContext.Teams.Add(team);
                }
                catch { }
            }
            this.coachBotContext.SaveChanges();

            foreach (var legacyChannel in config.Channels)
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
                    this.coachBotContext.Channels.Add(channel);
                }
                catch
                {

                }
            }
            this.coachBotContext.SaveChanges();

            return teams;
        }

        private void SetupAdministrator()
        {
            var player = new Player()
            {
                Name = "Thing'e'",
                SteamID = 76561197960374238,
                HubRole = PlayerHubRole.Owner,
                Rating = 7.2
            };

            this.coachBotContext.Players.Add(player);
            this.coachBotContext.SaveChanges();
        }

    }
}
