export interface FantasyTeamSummary {
    fantasyTeamId: number;
    fantasyTeamName: string;
    playerId: number;
    playerName: string;
    isComplete: boolean;
    fantasyTeamStatus: FantasyTeamStatus;
}

export enum FantasyTeamStatus {
    Open = 1,
    Pending = 2,
    Settled = 3
}
