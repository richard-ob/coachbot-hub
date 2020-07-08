import { MatchData } from '../../match-overview/model/match-data.interface';

export interface MatchStatistics {
    id: number;
    matchGoalsHome: number;
    matchGoalsAway: number;
    matchData: MatchData;
    createdDate: Date;
}
