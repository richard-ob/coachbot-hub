
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { FantasyTeam } from '../model/fantasy-team.model';
import { FantasyTeamSelection } from '../model/fantasy-team-selection.model';
import { FantasyPlayer } from '../model/fantasy-player.model';
import { PlayerStatisticFilters } from '../model/dtos/paged-player-statistics-request-dto.model';
import { PagedResult } from '../model/dtos/paged-result.model';
import { PlayerStatisticsFilterHelper } from '../model/helpers/player-statistics-filter.helper';
import { Tournament } from '../model/tournament.model';
import { FantasyTeamRank } from '../model/fantasy-team-rank.model';
import { FantasyPlayerRank } from '../model/fantasy-player-rank';
import { FantasyTeamSummary } from '../model/fantasy-team-summary.model';

@Injectable({
    providedIn: 'root'
})
export class FantasyService {

    constructor(private http: HttpClient) { }

    getFantasyTeams(tournamentId: number): Observable<FantasyTeam[]> {
        return this.http.get<FantasyTeam[]>(`${environment.apiUrl}/api/fantasy/tournament/${tournamentId}`);
    }

    getFantasyTeamsForUser(): Observable<FantasyTeamSummary[]> {
        return this.http.get<FantasyTeamSummary[]>(`${environment.apiUrl}/api/fantasy/teams/@me`);
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
            `${environment.apiUrl}/api/fantasy/tournament/${filters.tournamentId}/players`,
            PlayerStatisticsFilterHelper.generatePlayerStatisticsFilter(page, pageSize, sortBy, sortOrder, filters)
        );
    }

    generateFantasySnapshots(tournamentId: number): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournaments/${tournamentId}/generate-fantasy-snapshots`, null);
    }

    getAvailableFantasyTournamentsForUser(): Observable<Tournament[]> {
        return this.http.get<Tournament[]>(`${environment.apiUrl}/api/fantasy/tournament/available`);
    }

    getFantasyTeamRankings(tournamentId: number): Observable<FantasyTeamRank[]> {
        return this.http.get<FantasyTeamRank[]>(`${environment.apiUrl}/api/fantasy/tournament/${tournamentId}/rankings`);
    }

    getFantasyPlayerRankings(tournamentId: number): Observable<FantasyPlayerRank[]> {
        return this.http.get<FantasyPlayerRank[]>(`${environment.apiUrl}/api/fantasy/tournament/${tournamentId}/player-rankings`);
    }

    getFantasyTeamSpotlight(tournamentId: number): Observable<FantasyTeamRank> {
        return this.http.get<FantasyTeamRank>(
            `${environment.apiUrl}/api/fantasy/tournament/${tournamentId}/current-phase-spotlight-team`
        );
    }

    getFantasyPlayerSpotlight(tournamentId: number): Observable<FantasyPlayerRank> {
        return this.http.get<FantasyPlayerRank>(
            `${environment.apiUrl}/api/fantasy/tournament/${tournamentId}/current-phase-spotlight-player`
        );
    }

    getFantasyTeamSummaries(tournamentId: number): Observable<FantasyTeamSummary[]> {
        return this.http.get<FantasyTeamSummary[]>(
            `${environment.apiUrl}/api/fantasy/tournament/${tournamentId}/team-summaries`
        );
    }

    getFantasyTeamSummary(fantasyTeamId: number): Observable<FantasyTeamSummary> {
        return this.http.get<FantasyTeamSummary>(
            `${environment.apiUrl}/api/fantasy/${fantasyTeamId}/summary`
        );
    }

}
