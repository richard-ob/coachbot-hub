import { TimePeriod } from '../time-period.enum';

export class PagedPlayerStatisticsRequestDto {
    page = 1;
    pageSize = 10;
    sortBy: string;
    sortOrder: string;
    timePeriod: TimePeriod;
}
