
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Match } from '../model/match.model';
import { PagedMatchRequestDto } from '../model/dtos/paged-match-request-dto.model';

@Injectable({
    providedIn: 'root'
})
export class MatchService {

    constructor(private http: HttpClient) { }

    getMatches(regionId: number, page: number): Observable<Match[]> {
        const pagedMatchRequestDto = new PagedMatchRequestDto();
        pagedMatchRequestDto.page = page;
        pagedMatchRequestDto.regionId = regionId;

        return this.http.post<Match[]>(`${environment.apiUrl}/api/match`, pagedMatchRequestDto);
    }

    getMatch(matchId: number): Observable<Match[]> {
        return this.http.get<Match[]>(`${environment.apiUrl}/api/match`);
    }
}
