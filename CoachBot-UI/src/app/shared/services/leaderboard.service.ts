import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Match } from '../../model/match';
import { Player } from '../../model/player';

@Injectable({
    providedIn: 'root'
})
export class LeaderboardService {

    constructor(private http: HttpClient) { }

    getPlayerLeaderboard(): Observable<Player[]> {
        return this.http.get<Player[]>(`http://localhost:5006/api/leaderboard/players/`).pipe();
    }

    getPlayerLeaderboardForChannel(channelId: string): Observable<Player[]> {
        return this.http.get<Player[]>(`http://localhost:5006/api/leaderboard/channel/${channelId}`).pipe();
    }
}
