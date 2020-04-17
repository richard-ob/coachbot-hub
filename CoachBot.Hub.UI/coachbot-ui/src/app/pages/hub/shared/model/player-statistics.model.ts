import { Player } from './player.model';

export interface PlayerStatistics {
    id: number;
    playerId: number;
    player: Player;
    createdDate: Date;
}
