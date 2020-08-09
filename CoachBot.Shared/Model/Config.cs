namespace CoachBot.Shared.Model
{
    public class Config
    {
        public string SteamApiToken { get; set; }

        public string SqlConnectionString { get; set; }

        public DiscordConfig DiscordConfig {get; set; }

        public BotConfig BotConfig { get; set; }

        public WebServerConfig WebServerConfig { get; set; }

        public AzureAssetsConfig AzureAssetsConfig { get; set; }
    }

    public class AzureAssetsConfig
    {
        public string AccountName { get; set; }

        public string Key { get; set; }

        public string ContainerName { get; set; }
    }

    public class WebServerConfig
    {
        public string ClientUrl { get; set; }

        public string HubApiUrl { get; set; }

        public int ApiPort { get; set; } = 80;

        public int SecureApiPort { get; set; } = 44380;

        public string SecurityCertFile { get; set; }

        public string SecurityCertPassword { get; set; }

        public int BotApiPort { get; set; } = 8080;
    }

    public class DiscordConfig
    {
        public string BotToken { get; set; }

        public string OAuth2Id { get; set; }

        public string OAuth2Secret { get; set; }

        public ulong OwnerGuildId { get; set; }

        public ulong AuditChannelId { get; set; } = 642879369366339635;

        public ulong ResultStreamChannelId { get; set; } = 741956156985180170;
    }

    public class BotConfig
    {
        public bool BotStealthMode { get; set; } = false;

        public bool EnableBotHubIntegration { get; set; } = false;

    }
}