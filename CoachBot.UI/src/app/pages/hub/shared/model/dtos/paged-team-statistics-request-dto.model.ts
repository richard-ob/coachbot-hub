import { TimePeriod } from '../time-period.enum';
import { TeamType } from '../team-type.enum';
import { MatchOutcomeType } from '../match-outcome-type.enum';
import { MatchFormat } from '../match-format.enum';

export class PagedTeamStatisticsRequestDto {
    page = 1;
    pageSize = 10;
    sortBy: string;
    sortOrder: string;
    filters: TeamStatisticFilters;
}

export class TeamStatisticFilters {
    timePeriod: TimePeriod = TimePeriod.AllTime;
    teamId: number;
    teamType: TeamType;
    matchOutcome?: MatchOutcomeType;
    matchFormat?: MatchFormat = MatchFormat.EightVsEight;
    tournamentId?: number;
    tournamentGroupId?: number;
    regionId: number;
    includeInactive = true;
}
