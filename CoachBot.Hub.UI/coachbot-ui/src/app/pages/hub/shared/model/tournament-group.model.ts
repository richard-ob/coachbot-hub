import { TournamentGroupTeam } from './tournament-group-team.model';

export class TournamentGroup {
    id: number;
    name: string;
    tournamentStageId: number;
    tournamentGroupTeams: TournamentGroupTeam;
}
