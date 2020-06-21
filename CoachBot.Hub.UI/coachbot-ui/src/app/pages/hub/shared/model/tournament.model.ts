import { MatchFormat } from './match-format.enum';
import { TournamentStage } from './tournament-stage.model';
import { TournamentSeries } from './tournament-series.model';

export class Tournament {
    id: number;
    isPublic: boolean;
    tournamentType: number;
    tournamentSeriesId: number;
    tournamentSeries: TournamentSeries;
    startDate?: Date = new Date();
    endDate?: Date;
    format: MatchFormat;
    fantasyPointsLimit: number;
    tournamentStages?: TournamentStage[];
}
