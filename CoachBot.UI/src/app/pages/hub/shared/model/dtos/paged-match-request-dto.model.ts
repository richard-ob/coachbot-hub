import { TimePeriod } from '../time-period.enum';
import { MatchTypes } from '../match-types.enum';

export class PagedMatchRequestDto {
    page = 1;
    pageSize = 10;
    sortBy: string;
    sortOrder: string;
    filters: MatchFilters;
}

export class MatchFilters {
    timePeriod: TimePeriod = TimePeriod.AllTime;
    matchType: MatchTypes;
    regionId?: number;
    playerId?: number;
    teamId?: number;
    tournamentId?: number;
    includeUpcoming = false;
    includePast = false;
    includeUnpublished = false;
    dateFrom?: Date;
    dateTo?: Date;
}
