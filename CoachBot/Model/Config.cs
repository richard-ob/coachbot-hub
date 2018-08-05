using System.Collections.Generic;

namespace CoachBot.Model
{
    public class Config
    {
        public string BotToken { get; set; }

        public string OAuth2Id { get; set; }

        public string OAuth2Secret { get; set; }

        public List<Server> Servers { get; set; }

        public List<Channel> Channels { get; set; }

    }
}
