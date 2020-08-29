
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { PagedResult } from '../model/dtos/paged-result.model';
import { TeamStatistics } from '../model/team-statistics.model';
import { TeamStatisticFilters } from '../model/dtos/paged-team-statistics-request-dto.model';
import { Team } from '../model/team.model';
import { PlayerTeamStatisticsTotals } from '../model/player-team-statistics-totals.model';
import { MatchDayTotals } from '../model/team-match-day-totals';
import { TeamStatisticsFilterHelper } from '../model/helpers/team-statistics-filter.helper';
import { TeamType } from '../model/team-type.enum';
import { TeamPerformanceSnapshot } from '../model/team-peformance-snapshot.model';
import { TeamMatchStatistics } from '../model/team-match-statistics.model';

@Injectable({
    providedIn: 'root'
})
export class TeamService {

    constructor(private http: HttpClient) { }

    getTeam(teamId: number): Observable<Team> {
        return this.http.get<Team>(`${environment.apiUrl}/api/team/${teamId}`);
    }

    getTeamByCode(teamCode: string, regionId: number): Observable<Team> {
        return this.http.get<Team>(`${environment.apiUrl}/api/team/code/${teamCode}/region/${regionId}`);
    }

    getTeams(regionId: number, teamType: TeamType = null): Observable<Team[]> {
        let params = {};

        if (teamType) {
            params = { teamType };
        }

        return this.http.get<Team[]>(`${environment.apiUrl}/api/team/region/${regionId}`, { params });
    }

    getTeamStatistics(
        page: number,
        pageSize = 10,
        sortBy: string = null,
        sortOrder: string = null,
        filters: TeamStatisticFilters
    ): Observable<PagedResult<TeamStatistics>> {
        return this.http.post<PagedResult<TeamStatistics>>(`${environment.apiUrl}/api/teamstatistics`,
            TeamStatisticsFilterHelper.generatePlayerStatisticsFilter(page, pageSize, sortBy, sortOrder, filters)
        );
    }

    getTeamMatchStatistics(
        page: number,
        pageSize = 10,
        sortBy: string = null,
        sortOrder: string = null,
        filters: TeamStatisticFilters
    ): Observable<PagedResult<TeamMatchStatistics>> {
        return this.http.post<PagedResult<TeamMatchStatistics>>(`${environment.apiUrl}/api/teamstatistics/matches`,
            TeamStatisticsFilterHelper.generatePlayerStatisticsFilter(page, pageSize, sortBy, sortOrder, filters)
        );
    }

    getMonthlyTeamPerformance(teamId: number): Observable<TeamPerformanceSnapshot[]> {
        return this.http.get<TeamPerformanceSnapshot[]>(`${environment.apiUrl}/api/teamstatistics/performance/monthly/${teamId}`);
    }

    getWeeklyTeamPerformance(teamId: number): Observable<TeamPerformanceSnapshot[]> {
        return this.http.get<TeamPerformanceSnapshot[]>(`${environment.apiUrl}/api/teamstatistics/performance/weekly/${teamId}`);
    }

    getDailyTeamPerformance(teamId: number): Observable<TeamPerformanceSnapshot[]> {
        return this.http.get<TeamPerformanceSnapshot[]>(`${environment.apiUrl}/api/teamstatistics/performance/daily/${teamId}`);
    }

    getContinuousTeamPerformance(teamId: number): Observable<TeamPerformanceSnapshot[]> {
        return this.http.get<TeamPerformanceSnapshot[]>(`${environment.apiUrl}/api/teamstatistics/performance/continuous/${teamId}`);
    }

    getTeamMatchDayTotals(teamId: number): Observable<MatchDayTotals[]> {
        return this.http.get<MatchDayTotals[]>(`${environment.apiUrl}/api/teamstatistics/match-totals/${teamId}`);
    }

    getTeamSquad(teamId: number): Observable<PlayerTeamStatisticsTotals[]> {
        return this.http.get<PlayerTeamStatisticsTotals[]>(`${environment.apiUrl}/api/team/${teamId}/squad`);
    }

    getTeamPlayerHistory(teamId: number): Observable<PlayerTeamStatisticsTotals[]> {
        return this.http.get<PlayerTeamStatisticsTotals[]>(`${environment.apiUrl}/api/team/${teamId}/player-history`);
    }

    updateTeam(team: Team) {
        return this.http.put<void>(`${environment.apiUrl}/api/team`, team);
    }

    createTeam(team: Team) {
        return this.http.post<void>(`${environment.apiUrl}/api/team`, team);
    }

    deleteTeam(teamId: number) {
        return this.http.delete<void>(`${environment.apiUrl}/api/team/${teamId}`);
    }

    updateGuildId(teamId: number, guildId: number) {
        const updateGuildIdDto = {
            teamId,
            guildId
        };
        return this.http.post<void>(`${environment.apiUrl}/api/team/update-guild-id`, updateGuildIdDto);
    }

}
