import { TimePeriod } from '../time-period.enum';
import { PositionGroup } from '../position-group.enum';
import { MatchTeamType } from '../match-team-type.enum';
import { MatchOutcomeType } from '../match-outcome-type.enum';
import { MatchFormat } from '../match-format.enum';

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
    matchTeamType?: MatchTeamType;
    positionId?: number;
    regionId?: number;
    tournamentId?: number;
    includeSubstituteAppearances = true;
    minimumSecondsPlayed?: number;
    matchOutcome?: MatchOutcomeType;
    matchFormat?: MatchFormat = MatchFormat.EightVsEight;
    dateFrom?: Date;
    dateTo?: Date;
    minimumRating?: number;
    maximumRating?: number;
    positionGroup?: PositionGroup;
    playerName?: string;
    positionName?: string;
    excludePlayers?: number[] = [];
}
