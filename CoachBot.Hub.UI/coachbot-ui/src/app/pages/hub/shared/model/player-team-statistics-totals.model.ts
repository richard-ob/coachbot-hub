import { PlayerTeam } from './player-team.model';
import { Position } from './position';
import { SteamUserProfile } from './steam-user-profile.model';

export interface PlayerTeamStatisticsTotals {
    playerTeam: PlayerTeam;
    position: Position;
    appearances: number;
    goals: number;
    assists: number;
    yellowCards: number;
    redCards: number;

    steamUserProfile: SteamUserProfile;
}
