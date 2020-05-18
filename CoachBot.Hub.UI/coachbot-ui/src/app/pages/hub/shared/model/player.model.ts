import { SteamUserProfile } from './steam-user-profile.model';
import { PlayerPosition } from './player-position.model';
import { PlayerTeam } from './player-team.model';

export interface Player {
    id: number;
    name: string;
    discordUserId: string;
    discordUserMention: string;
    steamID: string;
    disableDMNotifications: boolean;
    playerStatisticsTotalsId: number;
    playerStatisticsTotals: any;
    displayName: string;
    positions: PlayerPosition[];
    teams: PlayerTeam[];
    steamUserProfile: SteamUserProfile;
    createdDate: Date;
}

