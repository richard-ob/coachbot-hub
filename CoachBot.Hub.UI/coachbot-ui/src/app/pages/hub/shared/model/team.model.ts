import { Guild } from './guild';
import { Region } from './region.model';
import { Channel } from './channel.model';
import { TeamType } from '../../match-overview/model/team-type.enum';
import { AssetImage } from 'src/app/shared/models/asset-image.model';

export interface Team {
    id: number;
    name: string;
    teamCode: string;
    badgeImageId: number;
    badgeImage: AssetImage;
    kitEmote: null;
    badgeEmote: null;
    displayName: string;
    foundedDate: Date;
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
