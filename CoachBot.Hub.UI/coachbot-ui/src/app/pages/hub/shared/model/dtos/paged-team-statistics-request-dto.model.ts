import { TimePeriod } from '../time-period.enum';

export class PagedTeamStatisticsRequestDto {
    page = 1;
    pageSize = 10;
    sortOrder: string;
    timePeriod: TimePeriod;
}
