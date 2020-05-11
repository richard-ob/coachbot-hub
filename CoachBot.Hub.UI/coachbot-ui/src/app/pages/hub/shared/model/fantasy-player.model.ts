import { Player } from './player.model';
import { TournamentEdition } from './tournament-edition.model';
import { PositionGroup } from './position-group.enum';

export interface FantasyPlayer {
    id: number;
    playerId: number;
    player: Player;
    rating: number;
    position: PositionGroup;
    tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
}
