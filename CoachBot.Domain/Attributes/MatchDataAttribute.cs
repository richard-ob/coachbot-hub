using CoachBot.Domain.Model;
using System;

namespace CoachBot.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MatchDataStatistic : Attribute
    {
        public MatchDataTotalsType MatchDataTotalsType { get; private set; }
        public MatchDataStatisticType MatchDataStatisticType { get; private set; }

        public MatchDataStatistic(MatchDataStatisticType matchDataStatisticType, MatchDataTotalsType matchDataTotalsType = MatchDataTotalsType.None)
        {
            MatchDataTotalsType = matchDataTotalsType;
            MatchDataStatisticType = matchDataStatisticType;
        }
    }

    public enum MatchDataTotalsType
    {
        Aggregate,
        Average,
        None
    }
}
