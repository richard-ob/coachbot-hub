import { Server } from './server.model';
import { MatchStatistics } from './match-statistics.model';
import { Tournament } from './tournament.model';

export interface Match {
    id: number;
    teamHome: any;
    teamHomeId: number;
    teamAway: any;
    teamAwayId: number;
    serverId: number;
    server: Server;
    matchStatisticsId?: number;
    matchStatistics: MatchStatistics;
    tournamentId?: number;
    tournament?: Tournament;
    kickOff: Date;
    createdDate: Date;
}
