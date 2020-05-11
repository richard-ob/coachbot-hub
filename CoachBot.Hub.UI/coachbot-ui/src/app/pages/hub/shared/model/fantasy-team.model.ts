import { Player } from './player.model';
import { TournamentEdition } from './tournament-edition.model';
import { FantasyTeamSelection } from './fantasy-team-selection.model';

export class FantasyTeam {
    id: number;
    name: string;
    isFinalised: boolean;
    playerId: number;
    player: Player;
    tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
    fantasyTeamSelections: FantasyTeamSelection[];
}
