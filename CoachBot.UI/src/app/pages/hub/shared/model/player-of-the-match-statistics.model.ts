import { PositionGroup } from './position-group.enum';

export interface PlayerOfTheMatchStatistics {
    playerId: number;
    playerName: string;
    goals: number;
    goalsConceded: number;
    assists: number;
    keeperSaves: number;
    interceptions: number;
    passCompletion: number;
    positionGroup: PositionGroup;
}
