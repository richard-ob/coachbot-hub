import { PlayerTeam } from './player-team.model';
import { Position } from './position';

export interface PlayerTeamStatisticsTotals {
    playerTeam: PlayerTeam;
    position: Position;
    appearances: number;
    goals: number;
    assists: number;
    yellowCards: number;
    redCards: number;
}
