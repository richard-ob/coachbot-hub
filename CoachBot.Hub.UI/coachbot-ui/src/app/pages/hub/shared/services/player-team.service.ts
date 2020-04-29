
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

    getForPlayer(playerId: number, includeInactive = false): Observable<PlayerTeam[]> {
        const playerTeamRequestDto = {
            id: playerId,
            includeInactive
        };

        return this.http.post<PlayerTeam[]>(`${environment.apiUrl}/api/player-team/player`, playerTeamRequestDto);
    }

    getForTeam(teamId: number, includeInactive = false): Observable<PlayerTeam[]> {
        const playerTeamRequestDto = {
            id: teamId,
            includeInactive
        };

        return this.http.post<PlayerTeam[]>(`${environment.apiUrl}/api/player-team/team`, playerTeamRequestDto);
    }

    updatePlayerTeam(playerTeam: PlayerTeam): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/player-team`, playerTeam);
    }

}
