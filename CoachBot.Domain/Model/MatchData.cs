using CoachBot.Domain.Helpers;
using System.Collections.Generic;

namespace CoachBot.Domain.Model
{
    public class MatchData
    {
        public MatchDataMatchInfo MatchInfo { get; set; }
        public List<MatchDataTeam> Teams { get; set; }
        public List<MatchDataPlayer> Players { get; set; }
        public List<object> MatchEvents { get; set; }
    }

    public class MatchDataMatchInfo
    {
        public string Type { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Periods { get; set; }
        public string LastPeriodName { get; set; }
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
}
