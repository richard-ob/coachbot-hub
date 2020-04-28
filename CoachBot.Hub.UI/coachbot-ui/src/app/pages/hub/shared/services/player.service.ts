
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
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
import { PlayerAppearanceTotals } from '../model/player-appearance-totals.model';
import { PlayerPositionMatchStatistics } from '../model/player-match-statistics.model';

@Injectable({
    providedIn: 'root'
})
export class PlayerService {

    constructor(private http: HttpClient) { }

    getPlayers(page: number): Observable<PagedResult<Player>> {
        const pagedPlayerRequestDto = new PagedPlayerRequestDto();
        pagedPlayerRequestDto.page = page;

        return this.http.post<PagedResult<Player>>(`${environment.apiUrl}/api/player`, pagedPlayerRequestDto);
    }

    getPlayer(playerId: number): Observable<Player> {
        return this.http.get<Player>(`${environment.apiUrl}/api/player/${playerId}`);
    }

    getPlayerStatistics(page: number, sortBy: string = null, sortOrder: string, filters: PlayerStatisticFilters)
        : Observable<PagedResult<PlayerStatistics>> {
        return this.http.post<PagedResult<PlayerStatistics>>(
            `${environment.apiUrl}/api/player-statistics`, this.generatePlayerStatisticsFilter(page, sortBy, sortOrder, filters)
        );
    }

    getPlayerMatchStatistics(page: number, sortBy: string = null, sortOrder: string, filters: PlayerStatisticFilters)
        : Observable<PagedResult<PlayerPositionMatchStatistics[]>> {
        return this.http.post<PagedResult<PlayerPositionMatchStatistics[]>>(
            `${environment.apiUrl}/api/player-statistics/matches`, this.generatePlayerStatisticsFilter(page, sortBy, sortOrder, filters)
        );
    }

    getPlayerTeamStatisticsHistory(playerId: number): Observable<PlayerTeamStatisticsTotals[]> {
        return this.http.get<PlayerTeamStatisticsTotals[]>(`${environment.apiUrl}/api/player/${playerId}/team-history`);
    }

    getPlayerAppearanceTotals(playerId: number): Observable<PlayerAppearanceTotals[]> {
        return this.http.get<PlayerAppearanceTotals[]>(`${environment.apiUrl}/api/player-statistics/appearance-totals/${playerId}`);
    }

    getPlayerProfile(playerId: number): Observable<PlayerProfile> {
        return this.http.get<PlayerProfile>(`${environment.apiUrl}/api/player-profiles/${playerId}`);
    }

    getCurrentPlayer(): Observable<Player> {
        return this.http.get<Player>(`${environment.apiUrl}/api/player/@me`);
    }

    updateCurrentPlayer(player: Player) {
        return this.http.post<void>(`${environment.apiUrl}/api/player/@me`, player);
    }

    updateSteamId(steamId: string): Observable<void> {
        const steamIdDto = {
            steamId
        };

        return this.http.post<void>(`${environment.apiUrl}/api/player/update-steam-id`, steamIdDto);
    }

    private generatePlayerStatisticsFilter(page: number, sortBy: string, sortOrder: string, filters: PlayerStatisticFilters) {
        const pagedPlayerStatisticsRequestDto = new PagedPlayerStatisticsRequestDto();
        pagedPlayerStatisticsRequestDto.filters = filters;
        pagedPlayerStatisticsRequestDto.page = page;
        pagedPlayerStatisticsRequestDto.sortOrder = sortOrder;
        if (sortBy !== null) {
            pagedPlayerStatisticsRequestDto.sortBy = sortBy;
        }

        return pagedPlayerStatisticsRequestDto;
    }

}
