
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { FantasyTeam } from '../model/fantasy-team.model';
import { FantasyTeamSelection } from '../model/fantasy-team-selection.model';
import { FantasyPlayer } from '../model/fantasy-player.model';
import { TournamentEdition } from '../model/tournament-edition.model';

@Injectable({
    providedIn: 'root'
})
export class FantasyService {

    constructor(private http: HttpClient) { }

    getFantasyTeams(tournamentEditionId: number): Observable<FantasyTeam[]> {
        return this.http.get<FantasyTeam[]>(`${environment.apiUrl}/api/fantasy/tournament/${tournamentEditionId}`);
    }

    getFantasyTeam(fantasyTeamId: number): Observable<FantasyTeam> {
        return this.http.get<FantasyTeam>(`${environment.apiUrl}/api/fantasy/${fantasyTeamId}`);
    }

    createFantasyTeam(fantasyTeam: FantasyTeam): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/fantasy`, fantasyTeam);
    }

    updateFantasyTeam(fantasyTeam: FantasyTeam): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/fantasy`, fantasyTeam);
    }

    addFantasyTeamSelection(fantasyTeamSelection: FantasyTeamSelection): Observable<void> {
        return this.http.post<void>(
            `${environment.apiUrl}/api/fantasy/${fantasyTeamSelection.fantasyTeamId}/selections`, fantasyTeamSelection
        );
    }

    removeFantasyTeamSelection(fantasyTeamSelection: FantasyTeamSelection): Observable<void> {
        return this.http.delete<void>(
            `${environment.apiUrl}/api/fantasy/${fantasyTeamSelection.fantasyTeamId}/selections/${fantasyTeamSelection.id}`
        );
    }

    getFantasyPlayers(tournamentEditionId: number): Observable<FantasyPlayer[]> {
        return this.http.post<FantasyPlayer[]>(
            `${environment.apiUrl}/api/fantasy/tournament/${tournamentEditionId}/players`, { tournamentEditionId }
        );
    }

    getAvailableFantasyTournamentsForUser(): Observable<TournamentEdition[]> {
        return this.http.get<TournamentEdition[]>(`${environment.apiUrl}/api/fantasy/tournament/available`);
    }

}
