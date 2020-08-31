
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Match } from '../model/match.model';
import { PagedMatchRequestDto, MatchFilters } from '../model/dtos/paged-match-request-dto.model';
import { PagedResult } from '../model/dtos/paged-result.model';
import { PlayerOfTheMatchStatistics } from '../model/player-of-the-match-statistics.model';

@Injectable({
    providedIn: 'root'
})
export class MatchService {

    constructor(private http: HttpClient) { }

    getMatches(page: number, pageSize = 10, sortBy: string = null, sortOrder: string = null, filters: MatchFilters):
        Observable<PagedResult<Match>> {
        const pagedMatchRequestDto = new PagedMatchRequestDto();
        pagedMatchRequestDto.page = page;
        pagedMatchRequestDto.pageSize = pageSize;
        pagedMatchRequestDto.filters = filters;
        pagedMatchRequestDto.sortBy = sortBy;
        pagedMatchRequestDto.sortOrder = sortOrder;
        return this.http.post<PagedResult<Match>>(`${environment.apiUrl}/api/match`, pagedMatchRequestDto);
    }

    getMatch(matchId: number): Observable<Match> {
        return this.http.get<Match>(`${environment.apiUrl}/api/match/${matchId}`);
    }

    updateMatch(match: Match): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/match/${match.id}`, match);
    }

    getPlayerOfTheMatchStatistics(matchId: number): Observable<PlayerOfTheMatchStatistics> {
        return this.http.get<PlayerOfTheMatchStatistics>(`${environment.apiUrl}/api/match/${matchId}/player-of-the-match`);
    }

}
