using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Helpers
{
    public static class PositionGroupHelper
    {
        public static PositionGroup DeterminePositionGroup(IEnumerable<PlayerPositionMatchStatistics> playerPositionMatchStatistics)
        {
            var position = playerPositionMatchStatistics.OrderByDescending(p => p.SecondsPlayed).Select(p => p.Position.Name).First();

            return MapPositionToPositionGroup(position);
        }

        public static  PositionGroup MapPositionToPositionGroup(string positionName)
        {
            var defPositions = new string[] { "LWB", "LB", "LCB", "SWP", "CB", "RCB", "RB", "RWB" };
            var midPositions = new string[] { "LM", "LCM", "CDM", "CM", "CAM", "RCM", "RM" };
            var attackPositions = new string[] { "LW", "LF", "CF", "SS", "ST", "RF", "RW" };

            if (positionName == "GK")
            {
                return PositionGroup.Goalkeeper;
            }
            else if (defPositions.Any(p => p == positionName))
            {
                return PositionGroup.Defence;
            }
            else if (midPositions.Any(p => p == positionName))
            {
                return PositionGroup.Midfield;
            }
            else if (attackPositions.Any(p => p == positionName))
            {
                return PositionGroup.Attack;
            }

            return PositionGroup.Unknown;
        }

    }
}
