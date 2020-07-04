import { TournamentGroupTeam } from './tournament-group-team.model';
import { TournamentGroupMatch } from './tournament-group-match.model';

export class TournamentGroup {
    id: number;
    name: string;
    tournamentStageId: number;
    tournamentGroupTeams: TournamentGroupTeam[];
    tournamentGroupMatches: TournamentGroupMatch[];
}
