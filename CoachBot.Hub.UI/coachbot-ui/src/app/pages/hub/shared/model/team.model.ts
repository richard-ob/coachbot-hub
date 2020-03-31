import { Guild } from './guild';
import { Region } from './region.model';
import { Channel } from './channel.model';
import { TeamType } from '../../match-overview/model/team-type.enum';

export interface Team {
    id: number;
    name: string;
    teamCode: string;
    kitEmote: null;
    badgeEmote: null;
    displayName: string;
    teamType: TeamType;
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
