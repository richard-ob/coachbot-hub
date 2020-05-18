import { Player } from './player.model';
import { TournamentEdition } from './tournament-edition.model';
import { PositionGroup } from './position-group.enum';
import { Team } from './team.model';

export interface FantasyPlayer {
    id: number;
    playerId: number;
    player: Player;
    teamId: number;
    team: Team;
    rating: number;
    positionGroup: PositionGroup;
    tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
}
