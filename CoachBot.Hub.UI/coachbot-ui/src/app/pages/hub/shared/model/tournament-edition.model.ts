import { MatchFormat } from './match-format.enum';
import { TournamentStage } from './tournament-stage.model';

export class TournamentEdition {
    id: number;
    isPublic: boolean;
    tournamentId: number;
    startDate?: Date;
    endDate?: Date;
    format: MatchFormat;
    tournamentStages?: TournamentStage[];
}
