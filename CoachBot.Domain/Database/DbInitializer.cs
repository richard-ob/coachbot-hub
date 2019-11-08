using CoachBot.Model;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace CoachBot.Database
{
    public class DbInitializer
    {
        public static void Initialize(CoachBotContext context)
        {
            try
            {
               // JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            }
            catch
            {

            }
            context.Database.EnsureCreated();
        }
    }
}
