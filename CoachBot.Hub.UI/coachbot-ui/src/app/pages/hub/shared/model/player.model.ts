import { SteamUserProfile } from './steam-user-profile.model';

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
    steamUserProfile: SteamUserProfile;
    createdDate: Date;
}

