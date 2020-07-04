import { TimePeriod } from '../time-period.enum';
import { PositionGroup } from '../position-group.enum';

export class PagedPlayerStatisticsRequestDto {
    page = 1;
    pageSize = 10;
    sortBy: string;
    sortOrder: string;
    filters: PlayerStatisticFilters;
}

export class PlayerStatisticFilters {
    timePeriod: TimePeriod = TimePeriod.AllTime;
    playerId?: number;
    teamId?: number;
    channelId?: number;
    matchId?: number;
    positionId?: number;
    regionId?: number;
    tournamentId?: number;
    includeSubstituteAppearances = true;
    minimumSecondsPlayed?: number;
    dateFrom?: Date;
    dateTo?: Date;
    minimumRating?: number;
    maximumRating?: number;
    positionGroup?: PositionGroup;
    playerName?: string;
    positionName?: string;
    excludePlayers?: number[] = [];
}
