import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SteamUserProfileResponse } from '../../pages/hub/shared/model/steam-user-profile.model';

@Injectable({
    providedIn: 'root'
})
export class SteamService {

    constructor(private http: HttpClient) { }

    getUserProfiles(steamIds: string[]): Observable<SteamUserProfileResponse> {
        const steamApiUrl =
            'https://cors-anywhere.herokuapp.com/http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=C4307E3CAB21B7B98650B242A34E89F2&steamids=';
        const steamIdCsv = steamIds.join(',');

        return this.http.get<SteamUserProfileResponse>(`${steamApiUrl}${steamIdCsv}`);
    }

    getPlayingTime(steamId: string): Observable<any> {
        const steamApiUrl =
            'https://cors-anywhere.herokuapp.com/http://api.steampowered.com/IPlayerService/GetRecentlyPlayedGames/v0001/?key=C4307E3CAB21B7B98650B242A34E89F2&steamid=';

        return this.http.get<any>(`${steamApiUrl}${steamId}`);
    }

    getNicknames(steamId: string): Observable<string[]> {
        const steamApiUrl =
            'https://cors-anywhere.herokuapp.com/http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=C4307E3CAB21B7B98650B242A34E89F2&steamid=';

        return this.http.get<string[]>(`${steamApiUrl}${steamId}`);
    }

    getNews(): Observable<any> {
        const steamApiUrl =
            'https://cors-anywhere.herokuapp.com/https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?key=C4307E3CAB21B7B98650B242A34E89F2&appid=673560';

        return this.http.get<any>(steamApiUrl);
    }

}
