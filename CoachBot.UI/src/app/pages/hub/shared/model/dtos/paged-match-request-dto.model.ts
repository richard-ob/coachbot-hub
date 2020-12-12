import { TimePeriod } from '../time-period.enum';
import { MatchTypes } from '../match-types.enum';
import { MatchFormat } from '../match-format.enum';

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
    matchFormat: MatchFormat;
    regionId?: number;
    playerId?: number;
    teamId?: number;
    oppositionTeamId?: number;
    tournamentId?: number;
    includeUpcoming = false;
    includePast = false;
    includeUnpublished = false;
    includePlaceholders = false;
    dateFrom?: Date;
    dateTo?: Date;
}
