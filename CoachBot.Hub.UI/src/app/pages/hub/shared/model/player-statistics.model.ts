import { Player } from './player.model';
import { SteamUserProfile } from './steam-user-profile.model';

export interface PlayerStatistics {
    id: number;
    playerId: number;
    steamID: string;
    name: string;
    steamUserProfile: SteamUserProfile;
    createdDate: Date;
}
