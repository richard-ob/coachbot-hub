import { Team } from './team.model';

export class TournamentGroupTeam {
    id?: number;
    teamId: number;
    team?: Team;
    tournamentGroupId: number;
}
