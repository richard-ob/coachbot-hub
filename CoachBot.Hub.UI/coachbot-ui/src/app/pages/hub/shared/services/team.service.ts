
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { PagedResult } from '../model/dtos/paged-result.model';
import { TeamStatistics } from '../model/team-statistics.model';
import { PagedTeamStatisticsRequestDto } from '../model/dtos/paged-team-statistics-request-dto.model';
import { TimePeriod } from '../model/time-period.enum';
import { Team } from '../model/team.model';
import { PlayerTeamStatisticsTotals } from '../model/player-team-statistics-totals.model';
import { MatchDayTotals } from '../model/team-match-day-totals';

@Injectable({
    providedIn: 'root'
})
export class TeamService {

    constructor(private http: HttpClient) { }

    getTeam(teamId: number): Observable<Team> {
        return this.http.get<Team>(`${environment.apiUrl}/api/team/${teamId}`);
    }

    getTeams(regionId: number): Observable<Team[]> {
        return this.http.get<Team[]>(`${environment.apiUrl}/api/team/region/${regionId}`);
    }

    getTeamStatistics(
        page: number,
        pageSize = 10,
        tournamentEditionId: number = null,
        teamId: number = null,
        regionId: number = null,
        sortBy: string = null,
        sortOrder: string = null
    ): Observable<PagedResult<TeamStatistics>> {
        const pagedTeamStatisticsRequestDto = new PagedTeamStatisticsRequestDto();
        pagedTeamStatisticsRequestDto.page = page;
        pagedTeamStatisticsRequestDto.pageSize = pageSize;
        pagedTeamStatisticsRequestDto.timePeriod = TimePeriod.AllTime;
        pagedTeamStatisticsRequestDto.tournamentEditionId = tournamentEditionId;
        pagedTeamStatisticsRequestDto.teamId = teamId;
        pagedTeamStatisticsRequestDto.regionId = regionId;
        pagedTeamStatisticsRequestDto.sortBy = sortBy;
        pagedTeamStatisticsRequestDto.sortOrder = sortOrder;

        return this.http.post<PagedResult<TeamStatistics>>(`${environment.apiUrl}/api/teamstatistics`, pagedTeamStatisticsRequestDto);
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

    updateGuildId(teamId: number, guildId: number) {
        const updateGuildIdDto = {
            teamId,
            guildId
        };
        return this.http.post<void>(`${environment.apiUrl}/api/team/update-guild-id`, updateGuildIdDto);
    }

}
