import { Pipe, PipeTransform } from '@angular/core';
import { SteamUserProfile } from '../../shared/model/steam-user-profile.model';
import * as SteamID from 'steamid';

@Pipe({ name: 'steamProfileFlag' })
export class SteamProfileFlagPipe implements PipeTransform {
    transform(steamUserProfiles: SteamUserProfile[], steamId: string): string {
        if (!steamUserProfiles) {
            return null;
        }

        if (steamId && steamId.length > 5) {
            const steamUser = new SteamID(steamId);
            const steamId64 = steamUser.getSteamID64();
            const steamUserProfile = steamUserProfiles.find(u => u.steamid === steamId64);
            if (steamUserProfile) {
                console.log(steamUserProfile.loccountrycode);
                return steamUserProfile.loccountrycode;
            }
        }

        return null;
    }
}
