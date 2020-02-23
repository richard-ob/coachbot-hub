
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { PagedResult } from '../model/dtos/paged-result.model';
import { PagedPlayerRequestDto } from '../model/dtos/paged-player-request-dto.model';
import { Player } from '../model/player.model';
import { PlayerStatistics } from '../model/player-statistics.model';
import { PagedPlayerStatisticsRequestDto } from '../model/dtos/paged-player-statistics-request-dto.model';
import { TimePeriod } from '../model/time-period.enum';

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

    getPlayerStatistics(page: number, sortBy: string = null, sortOrder: string, timePeriod: number = 0)
        : Observable<PagedResult<PlayerStatistics>> {
        const pagedPlayerStatisticsRequestDto = new PagedPlayerStatisticsRequestDto();
        pagedPlayerStatisticsRequestDto.page = page;
        pagedPlayerStatisticsRequestDto.timePeriod = timePeriod;
        pagedPlayerStatisticsRequestDto.sortOrder = sortOrder;
        if (sortBy !== null) {
            pagedPlayerStatisticsRequestDto.sortBy = sortBy;
        }

        return this.http.post<PagedResult<PlayerStatistics>>(`${environment.apiUrl}/api/playerstatistics`, pagedPlayerStatisticsRequestDto);
    }

}