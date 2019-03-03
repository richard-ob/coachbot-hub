import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Player } from '../../model/player';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class LeaderboardService {

    constructor(private http: HttpClient) { }

    getPlayerLeaderboard(): Observable<Player[]> {
        return this.http.get<Player[]>(`${environment.apiUrl}/api/leaderboard/players/`).pipe();
    }

    getPlayerLeaderboardForChannel(channelId: string): Observable<Player[]> {
        return this.http.get<Player[]>(`${environment.apiUrl}/api/leaderboard/channel/${channelId}`).pipe();
    }

    getLeaderboardForPlayer(userId: string): Observable<any[]> {
        return this.http.get<Player[]>(`${environment.apiUrl}/api/leaderboard/player/${userId}`).pipe();
    }
}
