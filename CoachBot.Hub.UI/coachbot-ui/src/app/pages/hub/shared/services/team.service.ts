
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { PagedResult } from '../model/dtos/paged-result.model';
import { TeamStatistics } from '../model/team-statistics.model';
import { PagedTeamStatisticsRequestDto } from '../model/dtos/paged-team-statistics-request-dto.model';
import { TimePeriod } from '../model/time-period.enum';
import { Team } from '../model/team.model';

@Injectable({
    providedIn: 'root'
})
export class TeamService {

    constructor(private http: HttpClient) { }

    getTeam(teamId: number): Observable<Team> {
        return this.http.get<Team>(`${environment.apiUrl}/api/team/${teamId}`);
    }

    getTeams(): Observable<Team[]> {
        return this.http.get<Team[]>(`${environment.apiUrl}/api/team`);
    }

    getTeamStatistics(page: number): Observable<PagedResult<TeamStatistics>> {
        const pagedTeamStatisticsRequestDto = new PagedTeamStatisticsRequestDto();
        pagedTeamStatisticsRequestDto.page = page;
        pagedTeamStatisticsRequestDto.timePeriod = TimePeriod.AllTime;

        return this.http.post<PagedResult<TeamStatistics>>(`${environment.apiUrl}/api/teamstatistics`, pagedTeamStatisticsRequestDto);
    }

    updateTeam(team: Team) {
        return this.http.put<void>(`${environment.apiUrl}/api/team`, team);
    }

}
