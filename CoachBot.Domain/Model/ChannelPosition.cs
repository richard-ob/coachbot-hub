using CoachBot.Model;

namespace CoachBot.Domain.Model
{
    public class ChannelPosition
    {
        public int ChannelId { get; set; }

        public Channel Channel { get; set; }

        public int PositionId { get; set; }

        public Position Position { get; set; }

    }
}
