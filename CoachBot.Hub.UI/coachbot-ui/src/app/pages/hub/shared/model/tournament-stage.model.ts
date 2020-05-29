import { TournamentEdition } from './tournament-edition.model';
import { TournamentPhase } from './tournament-phase.model';
import { TournamentGroup } from './tournament-group.model';

export interface TournamentStage {
    id?: number;
    name: string;
    tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
    tournamentPhases: TournamentPhase[];
    tournamentGroups: TournamentGroup[];
}
