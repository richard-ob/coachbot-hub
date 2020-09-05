import { Server } from './server.model';
import { MatchStatistics } from './match-statistics.model';
import { Tournament } from './tournament.model';
import { Map } from './map.model';
import { Player } from '@pages/hub/match-overview/model/match-data.interface';

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
    playerOfTheMatchId: number;
    playerOfTheMatch: Player;
    tournamentId?: number;
    tournament?: Tournament;
    kickOff: Date;
    map: Map;
    mapId: number;
    createdDate: Date;
}
