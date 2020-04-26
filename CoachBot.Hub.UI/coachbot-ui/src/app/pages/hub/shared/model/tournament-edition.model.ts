import { MatchFormat } from './match-format.enum';

export class TournamentEdition {
    id: number;
    isPublic: boolean;
    tournamentId: number;
    startDate?: Date;
    endDate?: Date;
    format: MatchFormat;
}
