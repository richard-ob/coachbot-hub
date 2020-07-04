using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.LegacyImporter.Model;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.LegacyImporter.Data
{
    public static class PositionsData
    {
        public static string[] PositionNumbers => new string[] {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"
        };

        public static string[] Positions => new string[] {
            "GK",
            "LB",
            "LCB",
            "CB",
            "RCB",
            "RB",
            "LW",
            "LCM",
            "CM",
            "RCM",
            "RW",
            "CF",
        };

        public static string[] OneVsOne => new string[] {
             "GK"
        };

        public static string[] TwoVsTwo => new string[] {
             "GK", "CM"
        };

        public static string[] ThreeVsThree => new string[] {
             "GK", "LM", "RM"
        };

        public static string[] FourVsFour => new string[] {
            "GK", "CB", "LM", "RM"
        };

        public static string[] FiveVsFive => new string[] {
            "GK", "CB", "LM", "RM", "CF"
        };

        public static string[] SixVsSix => new string[] {
            "GK", "LB", "RB", "CM", "LW", "RW"
        };

        public static string[] SevenVsSeven => new string[] {
            "GK", "LB", "RB", "CM", "LW", "CF", "RW"
        };

        public static string[] EightVsEight => new string[] {
            "GK", "LB", "CB", "RB", "CM", "LW", "CF", "RW"
        };

        public static string[] NineVsNine => new string[] {
            "GK", "LB", "CB", "RB", "LCM", "RCM", "LW", "CF", "RW"
        };

        public static string[] TenVsTen => new string[] {
            "GK", "LB", "CB", "RB", "LW", "LCM", "CM", "RCM", "RW", "CF"
        };

        public static string[] ElevenVsEleven => new string[] {
           "GK", "LB", "LCB", "RCB", "RB", "LW", "LCM", "CM", "RCM", "RW", "CF"
        };

        public static string[] GetPositionsBySize(int positions)
        {
            switch (positions)
            {
                case 1:
                    return OneVsOne;

                case 2:
                    return TwoVsTwo;

                case 3:
                    return ThreeVsThree;

                case 4:
                    return FourVsFour;

                case 5:
                    return FiveVsFive;

                case 6:
                    return SixVsSix;

                case 7:
                    return SevenVsSeven;

                case 8:
                    return EightVsEight;

                case 9:
                    return NineVsNine;

                case 10:
                    return TenVsTen;

                case 11:
                    return ElevenVsEleven;
            }

            return new string[] { };
        }

        public static ICollection<ChannelPosition> GenerateChannelPositions(List<LegacyPosition> legacyPositions, int channelId, CoachBotContext coachBotContext)
        {
            var positions = new List<ChannelPosition>();

            if (legacyPositions.Any(p => p.PositionName == "1"))
            {
                for (int i = 1; i <= legacyPositions.Count; i++)
                {
                    var channelPosition = new ChannelPosition()
                    {
                        PositionId = GetPositionId(coachBotContext, i.ToString()),
                        Ordinal = i,
                        ChannelId = channelId
                    };
                    positions.Add(channelPosition);
                }
            }
            else
            {
                var currentPositionOrdinal = 0;
                foreach (var position in GetPositionsBySize(legacyPositions.Count))
                {
                    var channelPosition = new ChannelPosition()
                    {
                        PositionId = GetPositionId(coachBotContext, position),
                        Ordinal = currentPositionOrdinal++,
                        ChannelId = channelId
                    };
                    positions.Add(channelPosition);
                }
            }

            return positions;
        }

        public static int GetPositionId(CoachBotContext coachBotContext, string position)
        {
            return coachBotContext.Positions.Single(p => p.Name == position).Id;
        }
    }
}