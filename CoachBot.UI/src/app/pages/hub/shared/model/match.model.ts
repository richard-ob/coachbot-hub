import { Server } from './server.model';
import { MatchStatistics } from './match-statistics.model';

export interface Match {
    id: number;
    teamHome: any;
    teamHomeId: number;
    teamAway: any;
    teamAwayId: number;
    serverId: number;
    server: Server;
    matchStatistics: MatchStatistics;
    kickOff: Date;
    createdDate: Date;
}
