import { TimePeriod } from '../time-period.enum';

export class PagedMatchRequestDto {
    page = 1;
    pageSize = 10;
    filters: MatchFilters;
}

export class MatchFilters {
    timePeriod: TimePeriod = TimePeriod.AllTime;
    regionId?: number;
    playerId?: number;
    teamId?: number;
    tournamentEditionId?: number;
    includeUpcoming = false;
    includePast = false;
    includeUnpublished = false;
    dateFrom?: Date;
    dateTo?: Date;
}
