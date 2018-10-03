import { Team } from './team';
import { Formation } from './formation';
import { Position } from './position';
import { Region } from './region';

export class Channel {
    id: any;
    idString: string;
    positions: Position[];
    team1: Team;
    team2: Team;
    formation: Formation;
    classicLineup: boolean;
    name: string;
    guildName: string;
    emotes: any[];
    isMixChannel: boolean;
    region: Region;
}
