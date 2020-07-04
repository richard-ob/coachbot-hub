import { Player } from './player.model';
import { Position } from './position';

export interface PlayerPosition {
    playerId: number;
    player?: Player;
    positionId: number;
    position?: Position;
}
