import { MatchFormat } from './match-format.enum';
import { TournamentStage } from './tournament-stage.model';
import { TournamentSeries } from './tournament-series.model';
import { TournamentStaff } from './tournament-staff.model';
import { TeamType } from './team-type.enum';
import { Team } from './team.model';

export class Tournament {
    id: number;
    name: string;
    isPublic: boolean;
    tournamentType: number;
    tournamentSeriesId: number;
    tournamentSeries: TournamentSeries;
    teamType: TeamType;
    startDate?: Date = new Date();
    endDate?: Date;
    format: MatchFormat;
    fantasyPointsLimit: number;
    tournamentStages?: TournamentStage[];
    tournamentStaff: TournamentStaff[];
    winningTeam: Team;
    hasStarted: boolean;
    hasEnded: boolean;
}
