
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { PlayerTeam } from '../model/player-team.model';

@Injectable({
    providedIn: 'root'
})
export class PlayerTeamService {

    constructor(private http: HttpClient) { }

    getForPlayer(playerId: number): Observable<PlayerTeam[]> {
        return this.http.get<PlayerTeam[]>(`${environment.apiUrl}/api/player-team/player/${playerId}`);
    }

    getForTeam(teamId: number): Observable<PlayerTeam[]> {
        return this.http.get<PlayerTeam[]>(`${environment.apiUrl}/api/player-team/team/${teamId}`);
    }

}
