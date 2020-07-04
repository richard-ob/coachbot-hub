import { Player } from './player.model';
import { FantasyTeamSelection } from './fantasy-team-selection.model';
import { Tournament } from './tournament.model';

export class FantasyTeam {
    id: number;
    name: string;
    isComplete: string;
    playerId: number;
    player: Player;
    tournamentId: number;
    tournament: Tournament;
    fantasyTeamSelections: FantasyTeamSelection[];
}
