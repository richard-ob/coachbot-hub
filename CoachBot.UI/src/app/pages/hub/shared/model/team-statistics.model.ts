import { Channel } from './channel.model';

export interface TeamStatistics {
    id: number;
    teamId: number;
    channel: Channel;
    statisticsTotals: any;
    createdDate: Date;
    badgeImage: string;
    channelId: number;
    teamName: string;
    redCards: number;
    redCardsAverage: number;
    yellowCards: number;
    yellowCardsAverage: number;
    fouls: number;
    foulsAverage: number;
    foulsSuffered: number;
    foulsSufferedAverage: number;
    slidingTacklesAverage: number;
    slidingTacklesCompletedAverage: number;
    goalsConceded: number;
    goalsConcededAverage: number;
    goals: number;
    goalsAverage: number;
    ownGoals: number;
    ownGoalsAverage: number;
    assists: number;
    assistsAverage: number;
    shots: number;
    shotsAverage: number;
    shotsOnGoal: number;
    shotsOnGoalAverage: number;
    shotAccuracyPercentage: number;
    passes: number;
    passesAverage: number;
    passesCompleted: number;
    passesCompletedAverage: number;
    passCompletionPercentageAverage: number;
    interceptions: number;
    interceptionsAverage: number;
    offsides: number;
    offsidesAverage: number;
    freeKicks: number;
    freeKicksAverage: number;
    penalties: number;
    penaltiesAverage: number;
    corners: number;
    throwIns: number;
    keeperSaves: number;
    keeperSavesAverage: number;
    keeperSavesCaughtAverage: number;
    goalKicksAverage: number;
    possessionAverage: number;
    distanceCoveredAverage: number;
    possessionPercentageAverage: number;
    goalDifference: number;
    points: number;
    appearances: number;
    substituteAppearances: number;
    wins: number;
    draws: number;
    losses: number;
    form: number[];
}
