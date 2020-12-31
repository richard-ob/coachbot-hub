import { TimePeriod } from '../time-period.enum';
import { TeamType } from '../team-type.enum';
import { MatchOutcomeType } from '../match-outcome-type.enum';
import { MatchFormat } from '../match-format.enum';
import { MatchTeamType } from '../match-team-type.enum';
import { MatchTypes } from '../match-types.enum';

export class PagedTeamStatisticsRequestDto {
    page = 1;
    pageSize = 10;
    sortBy: string;
    sortOrder: string;
    filters: TeamStatisticFilters;
}

export class TeamStatisticFilters {
    timePeriod: TimePeriod = TimePeriod.AllTime;
    teamId?: number;
    oppositionTeamId?: number;
    headToHead?: boolean;
    teamType: TeamType;
    matchOutcome?: MatchOutcomeType;
    matchFormat?: MatchFormat = MatchFormat.EightVsEight;
    matchType?: MatchTypes;
    tournamentId?: number;
    tournamentGroupId?: number;
    regionId: number;
    minimumMatches?: number;
    includeInactive = true;
}
