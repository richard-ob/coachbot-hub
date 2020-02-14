import { Server } from './server.model';
import { MatchStatistics } from './match-statistics.model';

export interface Match {
    id: number;
    teamHome: any;
    teamAway: any;
    serverId: number;
    server: Server;
    matchStatistics: MatchStatistics;
    readiedDate: Date;
    createdDate: Date;
}
