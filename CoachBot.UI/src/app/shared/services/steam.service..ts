import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SteamUserProfileResponse } from '../../pages/hub/shared/model/steam-user-profile.model';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SteamService {

    constructor(private http: HttpClient) { }

    getUserProfiles(steamIds: string[]): Observable<SteamUserProfileResponse> {
        const url = `${environment.apiUrl}/api/steam-proxy/user-profiles?steamIdsCsv=${steamIds.join(',')}`;

        return this.http.get<SteamUserProfileResponse>(url);
    }

    getPlayingTime(steamId: string): Observable<any> {
        const url = `${environment.apiUrl}/api/steam-proxy/playing-time?steamId=${steamId}`;

        return this.http.get<any>(url);
    }

    getNicknames(steamId: string): Observable<string[]> {
        const url = `${environment.apiUrl}/api/steam-proxy/nicknames?steamId=${steamId}`;

        return this.http.get<string[]>(url);
    }

    getNews(): Observable<any> {
        const url = `${environment.apiUrl}/api/steam-proxy/news`;

        return this.http.get<any>(url);
    }

}
