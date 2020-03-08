import { SteamUserProfile } from './steam-user-profile.model';
import { PlayerPosition } from './player-position.model';

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
    steamUserProfile: SteamUserProfile;
    createdDate: Date;
}

