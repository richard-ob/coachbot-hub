namespace CoachBot.Database
{
    public static class CoachBotContextExtensions
    {
        public static void Initialize(this CoachBotContext context)
        {
            try
            {
                context.Searches.RemoveRange(context.Searches);
                context.SaveChanges();
            }
            catch
            {

            }
            context.Database.EnsureCreated();
        }
    }
}
