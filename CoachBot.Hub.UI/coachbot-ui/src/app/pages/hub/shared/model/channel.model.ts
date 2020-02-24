import { Formation } from './formation';
import { ChannelPosition } from './channel-position';
import { Guild } from './guild';
import { Team } from './team.model';

export interface Channel {
    id: number;
    teamId: number;
    team: Team;
    subTeamName: string;
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
    updatedDate: Date;
    createdDate: Date;
}
