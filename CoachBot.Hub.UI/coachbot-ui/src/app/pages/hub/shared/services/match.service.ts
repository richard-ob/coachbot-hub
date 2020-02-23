
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Match } from '../model/match.model';
import { PagedMatchRequestDto } from '../model/dtos/paged-match-request-dto.model';
import { PagedResult } from '../model/dtos/paged-result.model';

@Injectable({
    providedIn: 'root'
})
export class MatchService {

    constructor(private http: HttpClient) { }

    getMatches(regionId: number, page: number): Observable<PagedResult<Match>> {
        const pagedMatchRequestDto = new PagedMatchRequestDto();
        pagedMatchRequestDto.page = page;
        pagedMatchRequestDto.regionId = regionId;

        return this.http.post<PagedResult<Match>>(`${environment.apiUrl}/api/match`, pagedMatchRequestDto);
    }

    getMatchesForPlayer(regionId: number, playerId: number, page: number): Observable<PagedResult<Match>> {
        const pagedMatchRequestDto = new PagedMatchRequestDto();
        pagedMatchRequestDto.page = page;
        pagedMatchRequestDto.regionId = regionId;
        pagedMatchRequestDto.playerId = playerId;

        return this.http.post<PagedResult<Match>>(`${environment.apiUrl}/api/match`, pagedMatchRequestDto);
    }

    getMatch(matchId: number): Observable<Match> {
        return this.http.get<Match>(`${environment.apiUrl}/api/match/${matchId}`);
    }
}
