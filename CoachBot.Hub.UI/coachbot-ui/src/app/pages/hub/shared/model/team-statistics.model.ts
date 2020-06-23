import { Channel } from './channel.model';

export interface TeamStatistics {
    id: number;
    teamId: number;
    channel: Channel;
    statisticsTotals: any;
    createdDate: Date;
    badgeImage: string;
}
