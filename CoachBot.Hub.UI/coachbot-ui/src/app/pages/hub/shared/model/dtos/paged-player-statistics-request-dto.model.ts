import { TimePeriod } from '../time-period.enum';

export class PagedPlayerStatisticsRequestDto {
    page = 1;
    pageSize = 10;
    sortBy: string;
    sortOrder: string;
    filters: PlayerStatisticFilters;
}

export class PlayerStatisticFilters {
    timePeriod: TimePeriod = TimePeriod.AllTime;
    teamId?: number;
    channelId?: number;
    positionId?: number;
    includeSubstituteAppearances = true;
}
