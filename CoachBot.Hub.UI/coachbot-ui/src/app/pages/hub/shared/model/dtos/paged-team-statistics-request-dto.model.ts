import { TimePeriod } from '../time-period.enum';

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
    tournamentId: number;
    regionId: number;
    includeInactive = true;
}
