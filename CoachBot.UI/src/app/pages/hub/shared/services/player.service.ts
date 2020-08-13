
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { PagedResult } from '../model/dtos/paged-result.model';
import { PagedPlayerRequestDto } from '../model/dtos/paged-player-request-dto.model';
import { Player } from '../model/player.model';
import { PlayerStatistics } from '../model/player-statistics.model';
import { PagedPlayerStatisticsRequestDto, PlayerStatisticFilters } from '../model/dtos/paged-player-statistics-request-dto.model';
import { TimePeriod } from '../model/time-period.enum';
import { PlayerTeamStatisticsTotals } from '../model/player-team-statistics-totals.model';
import { PlayerProfile } from '../model/player-profile.model';
import { MatchDayTotals } from '../model/team-match-day-totals';
import { PlayerPositionMatchStatistics } from '../model/player-position-match-statistics.model';
import { PlayerStatisticsFilterHelper } from '../model/helpers/player-statistics-filter.helper';
import { PlayerPerformanceSnapshot } from '../model/player-performance-snapshot.model';
import { PlayerMatchStatistics } from '../model/player-match-statistics.model';
import { tap } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class PlayerService {

    currentPlayer: Player;

    constructor(private http: HttpClient) { }

    getCurrentPlayer(): Observable<Player> {
        return this.http.get<Player>(`${environment.apiUrl}/api/player/@me`).pipe(tap(player => this.currentPlayer = player));
    }

    getPlayers(page: number): Observable<PagedResult<Player>> {
        const pagedPlayerRequestDto = new PagedPlayerRequestDto();
        pagedPlayerRequestDto.page = page;

        return this.http.post<PagedResult<Player>>(`${environment.apiUrl}/api/player`, pagedPlayerRequestDto);
    }

    getPlayer(playerId: number): Observable<Player> {
        return this.http.get<Player>(`${environment.apiUrl}/api/player/${playerId}`);
    }

    getPlayerStatistics(page: number, pageSize = 10, sortBy: string = null, sortOrder: string, filters: PlayerStatisticFilters)
        : Observable<PagedResult<PlayerStatistics>> {
        return this.http.post<PagedResult<PlayerStatistics>>(
            `${environment.apiUrl}/api/player-statistics`,
            PlayerStatisticsFilterHelper.generatePlayerStatisticsFilter(page, pageSize, sortBy, sortOrder, filters)
        );
    }

    getPlayerPositionMatchStatistics(page: number, pageSize = 10, sortBy: string = null, sortOrder: string, filters: PlayerStatisticFilters)
        : Observable<PagedResult<PlayerPositionMatchStatistics>> {
        return this.http.post<PagedResult<PlayerPositionMatchStatistics>>(
            `${environment.apiUrl}/api/player-statistics/matches-by-position`,
            PlayerStatisticsFilterHelper.generatePlayerStatisticsFilter(page, pageSize, sortBy, sortOrder, filters)
        );
    }

    getPlayerMatchStatistics(page: number, pageSize = 10, sortBy: string = null, sortOrder: string, filters: PlayerStatisticFilters)
        : Observable<PagedResult<PlayerMatchStatistics>> {
        return this.http.post<PagedResult<PlayerMatchStatistics>>(
            `${environment.apiUrl}/api/player-statistics/matches`,
            PlayerStatisticsFilterHelper.generatePlayerStatisticsFilter(page, pageSize, sortBy, sortOrder, filters)
        );
    }

    getPlayerTeamStatisticsHistory(playerId: number): Observable<PlayerTeamStatisticsTotals[]> {
        return this.http.get<PlayerTeamStatisticsTotals[]>(`${environment.apiUrl}/api/player/${playerId}/team-history`);
    }

    getMonthlyPlayerPerformance(playerId: number): Observable<PlayerPerformanceSnapshot[]> {
        return this.http.get<PlayerPerformanceSnapshot[]>(`${environment.apiUrl}/api/player-statistics/performance/monthly/${playerId}`);
    }

    getWeeklyPlayerPerformance(playerId: number): Observable<PlayerPerformanceSnapshot[]> {
        return this.http.get<PlayerPerformanceSnapshot[]>(`${environment.apiUrl}/api/player-statistics/performance/weekly/${playerId}`);
    }

    getDailyPlayerPerformance(playerId: number): Observable<PlayerPerformanceSnapshot[]> {
        return this.http.get<PlayerPerformanceSnapshot[]>(`${environment.apiUrl}/api/player-statistics/performance/daily/${playerId}`);
    }

    getContinuousPlayerPerformance(playerId: number): Observable<PlayerPerformanceSnapshot[]> {
        return this.http.get<PlayerPerformanceSnapshot[]>(`${environment.apiUrl}/api/player-statistics/performance/continuous/${playerId}`);
    }

    getPlayerAppearanceTotals(playerId: number): Observable<MatchDayTotals[]> {
        return this.http.get<MatchDayTotals[]>(`${environment.apiUrl}/api/player-statistics/appearance-totals/${playerId}`);
    }

    getPlayerProfile(playerId: number): Observable<PlayerProfile> {
        return this.http.get<PlayerProfile>(`${environment.apiUrl}/api/player-profiles/${playerId}`);
    }

    searchPlayerByName(playerName: string): Observable<Player[]> {
        const params = new HttpParams().set('playerName', playerName);
        return this.http.get<Player[]>(`${environment.apiUrl}/api/player/search`, { params });
    }

    updateCurrentPlayer(player: Player) {
        return this.http.post<void>(`${environment.apiUrl}/api/player/@me`, player);
    }

    updatePlayerRating(player: Player) {
        return this.http.post<void>(`${environment.apiUrl}/api/player/update-rating`, player);
    }

    updateSteamId(steamId: string): Observable<void> {
        const steamIdDto = {
            steamId
        };

        return this.http.post<void>(`${environment.apiUrl}/api/player/update-steam-id`, steamIdDto);
    }

}
