
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

    getMatch(matchId: number): Observable<PagedResult<Match>> {
        return this.http.get<PagedResult<Match>>(`${environment.apiUrl}/api/match/${matchId}`);
    }
}
