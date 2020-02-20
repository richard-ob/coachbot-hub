import { Region } from './region.model';
import { Formation } from './formation';
import { ChannelPosition } from './channel-position';
import { Guild } from './guild';

export interface Channel {
    id: number;
    name: string;
    discordChannelId: string;
    discordChannelName: string;
    channelPositions: ChannelPosition[];
    color: string;
    formation: Formation;
    teamCode: string;
    guildId: number;
    guild: Guild;
    useClassicLineup: boolean;
    disableSearchNotifications: boolean;
    duplicityProtection: boolean;
    isMixChannel: boolean;
    inactive: boolean;
    region: Region;
    updatedDate: Date;
    createdDate: Date;
}
