import { FantasyPlayer } from './fantasy-player.model';
import { FantasyTeam } from './fantasy-team.model';

export interface FantasyTeamSelection {
    id: number;
    isFlex: boolean;
    fantasyPlayerId: number;
    fantasyPlayer: FantasyPlayer;
    fantasyTeamId: number;
    fantasyTeam: FantasyTeam;
}
