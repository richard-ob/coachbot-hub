
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { FantasyTeam } from '../model/fantasy-team.model';
import { FantasyTeamSelection } from '../model/fantasy-team-selection.model';
import { FantasyPlayer } from '../model/fantasy-player.model';
import { TournamentEdition } from '../model/tournament-edition.model';
import { PlayerStatisticFilters } from '../model/dtos/paged-player-statistics-request-dto.model';
import { PagedResult } from '../model/dtos/paged-result.model';
import { PlayerStatisticsFilterHelper } from '../model/helpers/player-statistics-filter.helper';

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

    getFantasyPlayers(page: number, pageSize = 10, sortBy: string = null, sortOrder: string, filters: PlayerStatisticFilters)
        : Observable<PagedResult<FantasyPlayer>> {
        return this.http.post<PagedResult<FantasyPlayer>>(
            `${environment.apiUrl}/api/fantasy/tournament/${filters.tournamentEditionId}/players`,
            PlayerStatisticsFilterHelper.generatePlayerStatisticsFilter(page, pageSize, sortBy, sortOrder, filters)
        );
    }

    getAvailableFantasyTournamentsForUser(): Observable<TournamentEdition[]> {
        return this.http.get<TournamentEdition[]>(`${environment.apiUrl}/api/fantasy/tournament/available`);
    }

}
