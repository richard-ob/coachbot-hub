
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { MatchStatistics } from '../model/match-statistics.model';

@Injectable({
    providedIn: 'root'
})
export class MatchStatisticsService {

    constructor(private http: HttpClient) { }

    submitMatchStatistics(matchId: number, matchStatistics: MatchStatistics): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/match-statistics/${matchId}`, matchStatistics);
    }

    createMatchFromMatchStatistics(matchStatisticsId: number): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/match-statistics/${matchStatisticsId}/create-match`, null);
    }

    swapTeams(matchStatisticsId: number): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/match-statistics/${matchStatisticsId}/swap-teams`, null);
    }

    getUnlinkedMatchStatistics(): Observable<MatchStatistics[]> {
        return this.http.get<MatchStatistics[]>(`${environment.apiUrl}/api/match-statistics/unlinked`);
    }

}
