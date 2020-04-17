using CoachBot.Domain.Attributes;
using CoachBot.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoachBot.Domain.Extensions
{
    public static class MatchStatisticsExtensions
    {
        public static void AddMatchDataStatistics(this MatchStatisticsBase teamMatchStatistics, List<int> matchDataStatistics)
        {
            PropertyInfo[] properties = typeof(MatchStatisticsBase).GetProperties();
            foreach (PropertyInfo property in properties.Where(p => p.GetCustomAttribute(typeof(MatchDataStatistic)) != null))
            {
                var matchDataAttribute = (MatchDataStatistic)property.GetCustomAttribute(typeof(MatchDataStatistic));
                var value = matchDataStatistics[(int)matchDataAttribute.MatchDataStatisticType];
                property.SetValue(teamMatchStatistics, value);
            }
        }
    }
}
