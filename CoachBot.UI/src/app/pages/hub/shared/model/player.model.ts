import { SteamUserProfile } from './steam-user-profile.model';
import { PlayerPosition } from './player-position.model';
import { PlayerTeam } from './player-team.model';
import { PlayerHubRole } from './player-hub-role.enum';
import { Country } from './country.model';

export interface Player {
    id: number;
    name: string;
    discordUserId: string;
    discordUserMention: string;
    steamID: string;
    countryId: number;
    country: Country;
    rating: number;
    disableDMNotifications: boolean;
    playerStatisticsTotalsId: number;
    playerStatisticsTotals: any;
    displayName: string;
    positions: PlayerPosition[];
    teams: PlayerTeam[];
    steamUserProfile: SteamUserProfile;
    hubRole: PlayerHubRole;
    playingSince: Date;
    createdDate: Date;
}

