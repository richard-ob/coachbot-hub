using CoachBot.Domain.Helpers;
using System.Collections.Generic;

namespace CoachBot.Domain.Model
{
    public class MatchData
    {
        public MatchDataMatchInfo MatchInfo { get; set; }
        public List<MatchDataTeam> Teams { get; set; }
        public List<MatchDataPlayer> Players { get; set; }
        public List<MatchEvent> MatchEvents { get; set; }
    }

    public class MatchEvent
    {
        public string Event { get; set; } // See MatchEventTypes constants
        public string Period { get; set; } 
        public string Player1SteamId { get; set; }
        public string Player2SteamId { get; set; }
        public int Second { get; set; }
        public string Team { get; set; }
    }

    public class MatchDataMatchInfo
    {
        public string Type { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Periods { get; set; }
        public string LastPeriodName { get; set; }
        public string MapName { get; set; }
        public MatchFormat Format { get; set; }
        public string ServerName { get; set; }
    }

    public class MatchDataTeamMatchTotal
    {
        public string Name { get; set; }
        public string Side { get; set; }
        public bool IsMix { get; set; }
        public List<int> Statistics { get; set; }
    }

    public class MatchDataMatchPeriod
    {
        public int Period { get; set; }
        public string PeriodName { get; set; }
        public int AnnouncedInjuryTimeSeconds { get; set; }
        public int ActualInjuryTimeSeconds { get; set; }
        public List<int> Statistics { get; set; }
    }

    public class MatchDataTeam
    {
        public MatchDataTeamMatchTotal MatchTotal { get; set; }
        public List<MatchDataMatchPeriod> MatchPeriods { get; set; }
    }

    public class MatchDataPlayerInfo
    {
        public string SteamId { get; set; }
        public ulong? SteamId64 => SteamIdHelper.ConvertSteamIDToSteamID64(SteamId);
        public string Name { get; set; }
    }

    public class MatchDataMatchPeriodInfo
    {
        public int StartSecond { get; set; }
        public int EndSecond { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public bool IsHomeTeam => Team.Equals(MatchDataSideConstants.Home);
        public bool IsAwayTeam => Team.Equals(MatchDataSideConstants.Away);
    }

    public class MatchDataMatchPeriodData
    {
        public MatchDataMatchPeriodInfo Info { get; set; }
        public List<int> Statistics { get; set; }
    }

    public class MatchDataPlayer
    {
        public MatchDataPlayerInfo Info { get; set; }
        public List<MatchDataMatchPeriodData> MatchPeriodData { get; set; }
    }

    public enum MatchDataTeamType
    {
        Home,
        Away
    }

    public static class MatchDataSideConstants
    {
        public const string Home = "home";
        public const string Away = "away";
    }

    public enum MatchDataStatisticType
    {
        RedCards = 0,
        YellowCards,
        Fouls,
        FoulsSuffered,
        SlidingTackles,
        SlidingTacklesCompleted,
        GoalsConceded,
        Shots,
        ShotsOnGoal,
        PassesCompleted,
        Interceptions,
        Offsides,
        Goals,
        OwnGoals,
        Assists,
        Passes,
        FreeKicks,
        Penalties,
        Corners,
        ThrowIns,
        KeeperSaves,
        GoalKicks,
        Possession,
        DistanceCovered,
        KeeperSavesCaught
    }

    public static class MatchEventTypes
    {
        public const string Goal = "GOAL";
        public const string OwnGoal = "OWN GOAL";
        public const string FreeKick = "FREE KICK";
        public const string GoalKick = "GOAL KICK";
        public const string KickOff = "KICK-OFF";
        public const string CornerKick = "CORNER KICK";
        public const string ThrowIn = "THROW-IN";
        public const string Foul = "FOUL";
        public const string Penalty = "PENALTY";
        public const string MatchEnd = "MATCH END";
        public const string ExtraTime = "EXTRA TIME";
        public const string Penalties = "PENALTIES";
        public const string WarmUp = "WARM-UP";
        public const string Offside = "OFFSIDE";
        public const string YellowCard = "YELLOW CARD";
        public const string SecondYellow = "SECOND YELLOW";
        public const string RedCard = "RED CARD";
        public const string Assist = "ASSIST";
        public const string DoubleTouch = "DOUBLE TOUCH";
        public const string HalfTime = "HALF-TIME";
        public const string Save = "SAVE";
        public const string Dribble = "DRIBBLE";
        public const string Pass = "PASS";
        public const string Interception = "INTERCEPTION";
        public const string Timeout = "TIMEOUT";
        public const string TimeoutPending = "TIMEOUT PENDING";
        public const string Miss = "MISS";
        public const string Advantage = "ADVANTAGE";
        public const string Celebration = "CELEBRATION";
    }
}