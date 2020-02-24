import { Guild } from './guild';
import { Region } from './region.model';
import { Channel } from './channel.model';

export interface Team {
    id: number;
    name: string;
    teamCode: string;
    kitEmote: null;
    badgeEmote: null;
    displayName: string;
    regionId: number;
    region: Region;
    guildId: number;
    guild: Guild;
    color: string;
    systemColor: any;
    inactive: boolean;
    channels: Channel[];
    createdDate: Date;
    updatedDate: Date;
}
