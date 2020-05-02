import { Channel } from './channel.model';

export interface TeamStatistics {
    id: number;
    playerId: number;
    channel: Channel;
    statisticsTotals: any;
    createdDate: Date;
    badgeImage: string;
}
