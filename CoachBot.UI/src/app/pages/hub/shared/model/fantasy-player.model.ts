import { Player } from './player.model';
import { PositionGroup } from './position-group.enum';
import { Team } from './team.model';
import { Tournament } from './tournament.model';

export interface FantasyPlayer {
    id: number;
    playerId: number;
    player: Player;
    teamId: number;
    team: Team;
    rating: number;
    positionGroup: PositionGroup;
    tournamentId: number;
    tournament: Tournament;
}
