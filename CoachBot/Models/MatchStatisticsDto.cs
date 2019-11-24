using System.Collections.Generic;

namespace CoachBot.Models.Dto
{
    public class MatchStatisticsDto
    {
        public MatchData MatchData { get; set; }
        public string Access_Token { get; set; }
    }

    public class MatchData
    {
        public MatchInfo MatchInfo { get; set; }
        public List<Team> Teams { get; set; }
        public List<Player> Players { get; set; }
        public List<object> MatchEvents { get; set; }        
    }

    public class MatchInfo
    {
        public string Type { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Periods { get; set; }
        public string LastPeriodName { get; set; }
    }

    public class TeamMatchTotal
    {
        public string Name { get; set; }
        public string Side { get; set; }
        public bool IsMix { get; set; }
        public List<int> Statistics { get; set; }
    }

    public class MatchPeriod
    {
        public int Period { get; set; }
        public string PeriodName { get; set; }
        public int AnnouncedInjuryTimeSeconds { get; set; }
        public int ActualInjuryTimeSeconds { get; set; }
        public List<int> Statistics { get; set; }
    }

    public class Team
    {
        public TeamMatchTotal MatchTotal { get; set; }
        public List<MatchPeriod> MatchPeriods { get; set; }
    }

    public class PlayerInfo
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
    }

    public class MatchPeriodInfo
    {
        public int StartSecond { get; set; }
        public int EndSecond { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
    }

    public class MatchPeriodData
    {
        public MatchPeriodInfo Info { get; set; }
        public List<int> Statistics { get; set; }
    }

    public class Player
    {
        public PlayerInfo Info { get; set; }
        public List<MatchPeriodData> MatchPeriodData { get; set; }
    }
}
