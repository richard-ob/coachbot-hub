import { Tournament } from './tournament.model';
import { TournamentPhase } from './tournament-phase.model';
import { TournamentGroup } from './tournament-group.model';

export interface TournamentStage {
    id?: number;
    name: string;
    tournamentId: number;
    tournament: Tournament;
    tournamentPhases: TournamentPhase[];
    tournamentGroups: TournamentGroup[];
}
