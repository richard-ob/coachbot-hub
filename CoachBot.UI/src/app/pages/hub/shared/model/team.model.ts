import { Guild } from './guild';
import { Region } from './region.model';
import { Channel } from './channel.model';
import { AssetImage } from 'src/app/shared/models/asset-image.model';
import { MatchOutcomeType } from './match-outcome-type.enum';
import { TeamType } from './team-type.enum';

export class Team {
    id: number;
    name: string;
    teamCode: string;
    badgeImageId: number;
    badgeImage: AssetImage;
    kitEmote: string;
    badgeEmote: string;
    displayName: string;
    foundedDate: Date;
    teamType: TeamType;
    regionId: number;
    region: Region;
    guildId?: number;
    guild: Guild;
    color: string;
    systemColor: any;
    inactive: boolean;
    channels: Channel[];
    form: MatchOutcomeType[];
    token?: string;
    createdDate: Date;
    updatedDate: Date;
}
