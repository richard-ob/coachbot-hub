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
        let url = `${environment.apiUrl}/api/steam-proxy/user-profiles?steamIdsCsv=${steamIds.join(',')}`;

        if (this.isSafari) {
            url = 'https://cors-anywhere.herokuapp.com/' + url;
        }

        return this.http.get<SteamUserProfileResponse>(url);
    }

    getPlayingTime(steamId: string): Observable<any> {
        let url = `${environment.apiUrl}/api/steam-proxy/playing-time?steamId=${steamId}`;

        if (this.isSafari) {
            url = 'https://cors-anywhere.herokuapp.com/' + url;
        }

        return this.http.get<any>(url);
    }

    getNicknames(steamId: string): Observable<string[]> {
        let url = `${environment.apiUrl}/api/steam-proxy/nicknames?steamId=${steamId}`;

        if (this.isSafari) {
            url = 'https://cors-anywhere.herokuapp.com/' + url;
        }

        return this.http.get<string[]>(url);
    }

    getNews(): Observable<any> {
        let url = `${environment.apiUrl}/api/steam-proxy/news`;

        if (this.isSafari) {
            url = 'https://cors-anywhere.herokuapp.com/' + url;
        }

        return this.http.get<any>(url);
    }

    get isSafari() {
        return /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
    }

}
