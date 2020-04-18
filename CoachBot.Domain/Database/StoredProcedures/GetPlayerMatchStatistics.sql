CREATE PROCEDURE [dbo].[GetPlayerMatchStatistics]
 @TeamId INT = NULL,
 @PlayerId INT = NULL,
 @PositionId INT = NULL,
 @TournamentEditionId INT = NULL,
 @DateFrom DATETIME = NULL,
 @DateTo DATETIME = NULL,
 @PageSize INT = NULL,
 @Page INT = NULL,
 @TotalRecords INT OUTPUT

AS
BEGIN

SELECT PlayerId,
       AVG(RedCards),
       AVG(YellowCards),
       AVG(Fouls),
       AVG(FoulsSuffered),
       AVG(SlidingTackles),
       AVG(SlidingTacklesCompleted),
       AVG(GoalsConceded),
       AVG(Shots),
       AVG(ShotsOnGoal),
       AVG(PassesCompleted),
       AVG(Interceptions),
       AVG(Offsides),
       AVG(Goals),
       AVG(OwnGoals),
       AVG(Assists),
       AVG(Passes),
       AVG(FreeKicks),
       AVG(Penalties),
       AVG(Corners),
       AVG(ThrowIns),
       AVG(KeeperSaves),
       AVG(GoalKicks),
       AVG(Possession),
       AVG(DistanceCovered),
       AVG(KeeperSavesCaught),
       AVG(SecondsPlayed),
       Nickname
FROM dbo.PlayerMatchStatistics
GROUP BY PlayerId, Nickname

END
GO;