using CoachBot.Domain.Attributes;
using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoachBot.Domain.Extensions
{
    public static class MatchStatisticTotalsExtensions
    {
        public static void AddMatchDataStatistics(this StatisticTotals statisticTotals, List<int> matchDataStatistics)
        {
            PropertyInfo[] properties = typeof(StatisticTotals).GetProperties();
            foreach (PropertyInfo property in properties.Where(p => p.GetCustomAttribute(typeof(MatchDataStatistic)) != null))
            {
                var matchDataAttribute = (MatchDataStatistic)property.GetCustomAttribute(typeof(MatchDataStatistic));
                var existingValue = (int)property.GetValue(statisticTotals);
                var valueToAdd = matchDataStatistics[(int)matchDataAttribute.MatchDataStatisticType];
                switch (matchDataAttribute.MatchDataTotalsType)
                {
                    case MatchDataTotalsType.Aggregate:
                        property.SetValue(statisticTotals, existingValue + valueToAdd);
                        break;
                    case MatchDataTotalsType.Average:
                        var newAggregateValue = (existingValue * statisticTotals.Appearances) + valueToAdd;
                        var newAverage = newAggregateValue / (statisticTotals.Appearances + 1);
                        property.SetValue(statisticTotals, newAverage);
                        break;
                    case MatchDataTotalsType.None:
                        property.SetValue(statisticTotals, valueToAdd);
                        break;
                };
            }
        }
    }
}
