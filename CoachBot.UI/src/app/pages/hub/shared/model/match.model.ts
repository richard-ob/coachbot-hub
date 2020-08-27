import { Server } from './server.model';
import { MatchStatistics } from './match-statistics.model';
import { Tournament } from './tournament.model';
import { Map } from './map.model';

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
    map: Map;
    mapId: number;
    createdDate: Date;
}
