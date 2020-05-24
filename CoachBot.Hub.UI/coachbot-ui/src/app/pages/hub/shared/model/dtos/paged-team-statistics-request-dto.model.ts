import { TimePeriod } from '../time-period.enum';

export class PagedTeamStatisticsRequestDto {
    page = 1;
    pageSize = 10;
    sortBy: string;
    sortOrder: string;
    timePeriod: TimePeriod;
    teamId: number;
    tournamentEditionId: number;
    regionId: number;
}
