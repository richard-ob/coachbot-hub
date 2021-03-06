export interface TeamPerformanceSnapshot {
    teamId: number;
    day: number;
    week: number;
    month: number;
    year: number;
    averageGoals: number;
    averageAssists: number;
    averageGoalsConceded: number;
    cleanSheets: number;
    appearances: number;
    wins: number;
    draws: number;
    losses: number;
}
