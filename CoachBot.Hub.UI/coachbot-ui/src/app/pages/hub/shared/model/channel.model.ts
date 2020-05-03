import { Formation } from './formation';
import { ChannelPosition } from './channel-position';
import { Guild } from './guild';
import { Team } from './team.model';

export class Channel {
    id?: number;
    teamId: number;
    team?: Team = null;
    subTeamName = null;
    discordChannelId = '';
    discordChannelName = '';
    channelPositions: ChannelPosition[] = [];
    formation: Formation = Formation.None;
    guildId?: number;
    guild: Guild = null;
    useClassicLineup = true;
    disableSearchNotifications = false;
    duplicityProtection = false;
    isMixChannel = false;
    searchIgnoreList = [];
    inactive = false;
    updatedDate: Date = new Date();
    createdDate: Date = new Date();
}
