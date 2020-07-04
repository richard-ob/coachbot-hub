import { TournamentGroupMatch } from './tournament-group-match.model';
import { TournamentStage } from './tournament-stage.model';

export interface TournamentPhase {
    id?: number;
    name: string;
    tournamentStageId: number;
    tournamentStage: TournamentStage;
    tournamentGroupMatches: TournamentGroupMatch[];
}
