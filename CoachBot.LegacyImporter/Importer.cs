using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.LegacyImporter.Data;
using CoachBot.LegacyImporter.Model;
using CoachBot.Model;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChannelType = CoachBot.Domain.Model.ChannelType;

namespace CoachBot.LegacyImporter
{
    public class Importer
    {
        private readonly DiscordSocketClient discordSocketClient;
        private readonly CoachBotContext coachBotContext;
        private readonly AssetImageService assetImageService;
        public readonly LegacyConfig config;
        public readonly List<LegacyMatch> matchHistory;

        public List<Region> Regions;
        public List<Server> Servers;
        public List<Position> Positions;
        public Dictionary<string, AssetImage> TeamAssetImages = new Dictionary<string, AssetImage>();

        public Importer(DiscordSocketClient discordSocketClient, CoachBotContext coachBotContext, AssetImageService assetImageService)
        {
            matchHistory = JsonConvert.DeserializeObject<List<LegacyMatch>>(File.ReadAllText(@"history.json"));
            config = JsonConvert.DeserializeObject<LegacyConfig>(File.ReadAllText(@"legacy-config.json"));
            this.discordSocketClient = discordSocketClient;
            this.coachBotContext = coachBotContext;
            this.assetImageService = assetImageService;
            this.Regions = GetRegions();
            this.Servers = GetServers();
            this.Positions = GetPositions();
        }

        public List<Region> GetRegions()
        {
            var regions = new List<Region>();

            foreach (var legacyRegion in config.Regions.Where(r => r.RegionName == "Europe").OrderBy(r => r.RegionId))
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

            foreach (var legacyServer in config.Servers.Where(s => s.RegionId == 1))
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
                    CountryId = ServersData.GetCorrectServerCountry(countryId, legacyServer.Name)
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
            this.coachBotContext.Positions.AddRange(PositionsData.Positions.ToList().Select(p => new Position() { Name = p }));
            this.coachBotContext.Positions.AddRange(PositionsData.PositionNumbers.ToList().Select(p => new Position() { Name = p }));
            this.coachBotContext.SaveChanges();

            return positions;
        }

        public List<Team> GetTeams()
        {
            var guilds = new List<Guild>();
            var teams = new List<Team>();
            var channels = new List<Channel>();

            var activeChannels = config.Channels
                .Where(c => c.RegionId == 1)
                .GroupBy(c => c.GuildName)
                .Select(s => new
                {
                    GuildName = s.Key,
                    ChannelID = s.Max(p => p.Id),
                    HasMatchCount = s.Count(p => matchHistory.Any(m => m.ChannelId == p.Id && m.MatchDate > DateTime.UtcNow.AddDays(-20)))
                })
                .Where(t => t.HasMatchCount > 0)
                .ToList();

            foreach (var guildChannel in activeChannels)
            {
                ulong channelId = guildChannel.ChannelID;
                if (guildChannel.GuildName == "Revolution")
                {
                    channelId = 613712939722997780;
                }

                if (guildChannel.GuildName == "False 11")
                {
                    channelId = 708347875733667870;
                }

                if (guildChannel.GuildName == "IOSoccer")
                {
                    channelId = 669659754548690974;
                }

                var discordChannel = this.discordSocketClient.GetChannel(channelId) as ITextChannel;

                try
                {
                    if (!string.IsNullOrEmpty(discordChannel.Guild.IconUrl))
                    {
                        var iconUrl = TeamBadges.GetBadgeImageUrl(discordChannel);
                        var assetImageContent = HttpImageRetrieval.GetImageAsBase64(iconUrl);
                        var assetImage = new AssetImage()
                        {
                            Base64EncodedImage = assetImageContent,
                            FileName = guildChannel.GuildName.Replace("#", "").Replace(" ", "-"),
                            Url = iconUrl,
                            CreatedById = 1
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
                try
                {
                    var leadChannel = config.Channels
                        .Where(c => c.RegionId == 1)
                        .OrderBy(c => c.LastSearch.HasValue)
                        .ThenBy(c => c.LastSearch)
                        .ThenBy(c => !string.IsNullOrWhiteSpace(c.Team1.BadgeEmote))
                        .First(c => c.GuildName == guild.Name);

                    if (leadChannel.GuildName == "Revolution")
                    {
                        leadChannel = config.Channels.First(c => c.Id == 613712939722997780);
                    }

                    if (leadChannel.GuildName == "False 11")
                    {
                        leadChannel = config.Channels.First(c => c.Id == 708347875733667870);
                    }

                    if (leadChannel.GuildName == "IOSoccer")
                    {
                        leadChannel = config.Channels.First(c => c.Id == 669659754548690974);
                    }

                    if (leadChannel.GuildName == "Portugal IOS")
                    {
                        leadChannel = config.Channels.First(c => c.Id == 616336914500288514);
                    }

                    var discordChannel = this.discordSocketClient.GetChannel(leadChannel.Id) as ITextChannel;

                    if (leadChannel == null)
                    {
                        continue;
                    }

                    if (TeamExclusions.Teams.Any(t => t == guild.Name))
                    {
                        continue;
                    }

                    var team = new Team()
                    {
                        TeamCode = TeamCodes.GetTeamCode(guild.Name),
                        Name = guild.Name,
                        BadgeEmote = BadgeEmote.GetBadgeEmote(guild.Name, leadChannel.Team1.BadgeEmote),
                        KitEmote = leadChannel.Team1.KitEmote,
                        RegionId = leadChannel.RegionId,
                        TeamType = TeamTypes.GetTeamTypeForTeam(guild.Name),
                        Color = TeamColours.GetColour(guild.Name, leadChannel.Team1.Color),
                        GuildId = guild.Id,
                        BadgeImageId = this.TeamAssetImages.Where(t => t.Key == guild.Name).Select(t => (int?)t.Value.Id).FirstOrDefault(),
                        FoundedDate = discordChannel.Guild.CreatedAt.UtcDateTime
                    };

                    if (team.TeamType != TeamType.Draft)
                    {
                        teams.Add(team);
                        this.coachBotContext.Teams.Add(team);
                    }
                }
                catch { }
            }
            this.coachBotContext.SaveChanges();

            foreach (var legacyChannel in config.Channels)
            {
                var discordChannel = this.discordSocketClient.GetChannel(legacyChannel.Id) as ITextChannel;

                if (discordChannel == null) continue;

                if (!matchHistory.Any(m => m.ChannelId == discordChannel.Id)) continue;

                if (ChannelExclusions.Channels.Any(c => c == legacyChannel.Id))
                {
                    continue;
                }

                var currentPosIndex = 0;

                try
                {
                    var teamId = teams.First(t => t.GuildId == guilds.First(g => g.Name == legacyChannel.GuildName).Id).Id;
                    var channel = new Channel()
                    {
                        TeamId = teamId,
                        DiscordChannelId = legacyChannel.Id,
                        DuplicityProtection = legacyChannel.EnableUnsignWhenPlayerStartsOtherGame,
                        ChannelType = legacyChannel.IsMixChannel ? ChannelType.PrivateMix : Domain.Model.ChannelType.Team,
                        DisableSearchNotifications = legacyChannel.DisableSearchNotifications,
                        UseClassicLineup = legacyChannel.ClassicLineup,
                        DiscordChannelName = legacyChannel.Name,
                        Formation = (Formation)legacyChannel.Formation,
                        UpdatedDate = matchHistory.Where(m => m.ChannelId == legacyChannel.Id).Max(m => m.MatchDate) 
                    };

                    channels.Add(channel);
                    this.coachBotContext.Channels.Add(channel);
                    this.coachBotContext.SaveChanges();
                    this.coachBotContext.ChannelPositions.AddRange(PositionsData.GenerateChannelPositions(legacyChannel.Positions, channel.Id, this.coachBotContext));
                    this.coachBotContext.SaveChanges();
                }
                catch
                {
                }
            }
            this.coachBotContext.SaveChanges();

            this.AddTeamsManually();

            this.assetImageService.GenerateAllAssetImageUrls();

            this.FixTeamNames();

            this.AddCaptains();

            return teams;
        }

        private void FixTeamNames()
        {
            var thcTeam = coachBotContext.Teams.Single(t => t.Name == "THC ҂ [ MultiGaming Team ]");
            thcTeam.Name = "Tiger Haxball Club";
            var ptTeam = coachBotContext.Teams.Single(t => t.Name == "Portugal IOS");
            ptTeam.Name = "Portugal";
            var frTeam = coachBotContext.Teams.Single(t => t.Name == "French Empire National Team IOS");
            frTeam.Name = "France";
            var czTeam = coachBotContext.Teams.Single(t => t.Name == "Czechoslovakia IOS");
            czTeam.Name = "Czechoslovakia";
            var pepTeam = coachBotContext.Teams.Single(t => t.Name == "Pepegas mix team");
            pepTeam.Name = "Pepega";

            coachBotContext.SaveChanges();
        }

        private void AddCaptains()
        {
            var thingePlayerTeam = new PlayerTeam()
            {
                TeamId = coachBotContext.Teams.Single(t => t.Name == "Excel").Id,
                IsPending = false,
                JoinDate = DateTime.UtcNow,
                TeamRole = TeamRole.Captain,
                PlayerId = 1
            };
            coachBotContext.PlayerTeams.Add(thingePlayerTeam);

            foreach (var captain in Captains.CaptainsList)
            {
                coachBotContext.Players.Add(captain.Player);
                var playerTeam = new PlayerTeam()
                {
                    TeamId = coachBotContext.Teams.Single(t => t.Name == captain.TeamName).Id,
                    IsPending = false,
                    JoinDate = DateTime.UtcNow,
                    TeamRole = TeamRole.Captain,
                    PlayerId = captain.Player.Id
                };
                coachBotContext.PlayerTeams.Add(playerTeam);
                coachBotContext.SaveChanges();
            }
        }

        private void AddTeamsManually()
        {
            foreach(var team in Teams.TeamSeed)
            {
                AssetImage assetImage = null;
                if (!string.IsNullOrWhiteSpace(team.BadgeUrl))
                {
                    var assetImageContent = HttpImageRetrieval.GetImageAsBase64(team.BadgeUrl);
                    assetImage = new AssetImage()
                    {
                        Base64EncodedImage = assetImageContent,
                        FileName = team.TeamCode + ".png",
                        Url = team.BadgeUrl,
                        CreatedById = 1
                    };
                    this.coachBotContext.AssetImages.Add(assetImage);
                }

                var discordChannel = this.discordSocketClient.GetChannel(team.ChannelID) as ITextChannel;

                var guild = new Guild()
                {
                    DiscordGuildId = discordChannel.GuildId,
                    Name = discordChannel.Guild.Name
                };
                if (!coachBotContext.Guilds.Any(g => g.DiscordGuildId == guild.DiscordGuildId))
                {
                    coachBotContext.Guilds.Add(guild);
                    coachBotContext.SaveChanges();
                }

                var newTeam = new Team()
                {
                    TeamCode = team.TeamCode,
                    Name = team.TeamName,
                    BadgeEmote = team.BadgeEmote,
                    RegionId = 1,
                    TeamType = team.TeamType,
                    Color = TeamColours.GetColour(team.TeamName, team.Colour),
                    GuildId = coachBotContext.Guilds.Single(g => g.DiscordGuildId == discordChannel.Guild.Id).Id,
                    BadgeImageId = assetImage != null ? assetImage.Id : (int?)null
                };
                coachBotContext.Teams.Add(newTeam);

                var channel = new Channel()
                {
                    ChannelType = ChannelType.Team,
                    DiscordChannelId = team.ChannelID,
                    DiscordChannelName = discordChannel.Name,
                    TeamId = newTeam.Id
                };
                coachBotContext.Channels.Add(channel);

                var legacyChannel = config.Channels.First(c => c.Id == team.ChannelID);
                this.coachBotContext.ChannelPositions.AddRange(PositionsData.GenerateChannelPositions(legacyChannel.Positions, channel.Id, this.coachBotContext));

                coachBotContext.SaveChanges();
            }
        }
    }

    public struct TeamSeed
    {
        public string TeamName { get; set; }
        public string TeamCode { get; set; }
        public ulong ChannelID { get; set; }
        public TeamType TeamType { get; set; }
        public string BadgeUrl { get; set; }
        public string BadgeEmote { get; set; }
        public string Colour { get; set; }
    }
}