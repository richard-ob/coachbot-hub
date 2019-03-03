import { Player } from './player';

export class Match {
    public channelId: number;
    public channelName: string;
    public players: Player[];
    public team1Name: string;
    public team2Name: string;
    public matchDate: Date;
}
