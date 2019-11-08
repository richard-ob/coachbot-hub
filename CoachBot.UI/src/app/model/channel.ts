import { Team } from './team';
import { Formation } from './formation';
import { ChannelPosition } from './channel-position';
import { Region } from './region';
import { Guild } from './guild';

export class Channel {
    id: any;
    idString: string;
    channelPositions: ChannelPosition[];
    color: string;
    team1: Team;
    team2: Team;
    formation: Formation;
    classicLineup: boolean;
    name: string;
    teamCode: string;
    guild: Guild;
    emotes: any[];
    isMixChannel: boolean;
    region: Region;
}
