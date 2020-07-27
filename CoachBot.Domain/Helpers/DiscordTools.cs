namespace CoachBot.Tools
{
    public static class DiscordTools
    {
        public static ulong ConvertMentionToUserID(string mention)
        {
            return ulong.Parse(mention.Replace("<@!", string.Empty).Replace(">", string.Empty));
        }

        public static bool IsMention(string mention)
        {
            return mention.StartsWith("<@!") && mention.EndsWith(">");
        }
    }
}