
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { TournamentGroupTeamDto } from '../model/dtos/tournament-group-team-dto.model';
import { TournamentGroup } from '../model/tournament-group.model';
import { Team } from '../model/team.model';
import { TournamentPhase } from '../model/tournament-phase.model';
import { TournamentMatchDaySlot } from '../model/tournament-match-day-slot.model';
import { TournamentSeries } from '../model/tournament-series.model';
import { Tournament } from '../model/tournament.model';
import { TournamentStaff } from '../model/tournament-staff.model';
import { Organisation } from '../model/organisation.model';
import { TournamentGroupStanding } from '../model/tournament-group-standing.model';

@Injectable({
    providedIn: 'root'
})
export class TournamentService {

    constructor(private http: HttpClient) { }

    getTournamentSeries(): Observable<TournamentSeries[]> {
        return this.http.get<TournamentSeries[]>(`${environment.apiUrl}/api/tournament-series`);
    }

    getTournamentSeriesById(tournamentSeriesId: number): Observable<TournamentSeries> {
        return this.http.get<TournamentSeries>(`${environment.apiUrl}/api/tournament-series/${tournamentSeriesId}`);
    }

    getTournaments(): Observable<Tournament[]> {
        return this.http.get<Tournament[]>(`${environment.apiUrl}/api/tournaments`);
    }

    getCurrentTournaments(): Observable<Tournament[]> {
        return this.http.get<Tournament[]>(`${environment.apiUrl}/api/tournaments/current`);
    }

    getPastTournaments(): Observable<Tournament[]> {
        return this.http.get<Tournament[]>(`${environment.apiUrl}/api/tournaments/past`);
    }

    getTournament(tournamentId: number): Observable<Tournament> {
        return this.http.get<Tournament>(`${environment.apiUrl}/api/tournaments/${tournamentId}`);
    }

    getTournamentOverview(tournamentId: number): Observable<Tournament> {
        return this.http.get<Tournament>(`${environment.apiUrl}/api/tournaments/${tournamentId}/overview`);
    }

    getTournamentsForTeam(teamId: number): Observable<Tournament[]> {
        return this.http.get<Tournament[]>(`${environment.apiUrl}/api/tournaments/team/${teamId}`);
    }

    createTournamentSeries(tournamentSeries: TournamentSeries): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournament-series`, tournamentSeries);
    }

    createTournament(tournament: Tournament): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournaments`, tournament);
    }

    updateTournament(tournament: Tournament): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/tournaments`, tournament);
    }

    createTournamentGroup(tournamentGroup: TournamentGroup): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournament-groups`, tournamentGroup);
    }

    deleteTournamentGroup(tournamentGroupId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/api/tournament-groups/${tournamentGroupId}`);
    }

    addTournamentGroupTeam(tournamentGroupTeamDto: TournamentGroupTeamDto): Observable<void> {
        return this.http.post<void>(
            `${environment.apiUrl}/api/tournament-groups/${tournamentGroupTeamDto.tournamentGroupId}/teams`, tournamentGroupTeamDto
        );
    }

    removeTournamentGroupTeam(teamId: number, tournamentGroupId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/api/tournament-groups/${tournamentGroupId}/teams/${teamId}`);
    }

    getTournamentStaff(tournamentId: number): Observable<TournamentStaff[]> {
        return this.http.get<TournamentStaff[]>(`${environment.apiUrl}/api/tournaments/${tournamentId}/staff`);
    }

    createTournamentStaff(tournamentStaff: TournamentStaff): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournament-staff`, tournamentStaff);
    }

    updateTournamentStaff(tournamentStaff: TournamentStaff): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/tournament-staff`, tournamentStaff);
    }

    deleteTournamentStaff(tournamentStaffId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/api/tournament-staff/${tournamentStaffId}`);
    }

    getTournamentTeams(tournamentId: number): Observable<Team[]> {
        return this.http.get<Team[]>(`${environment.apiUrl}/api/tournaments/${tournamentId}/teams`);
    }

    getCurrentPhase(tournamentId: number): Observable<TournamentPhase> {
        return this.http.get<TournamentPhase>(`${environment.apiUrl}/api/tournaments/${tournamentId}/current-phase`);
    }

    getTournamentMatchDaySlots(tournamentId: number): Observable<TournamentMatchDaySlot[]> {
        return this.http.get<TournamentMatchDaySlot[]>(
            `${environment.apiUrl}/api/tournaments/${tournamentId}/match-day-slots`
        );
    }

    createTournamentMatchDaySlot(tournamentMatchDaySlot: TournamentMatchDaySlot) {
        return this.http.post<TournamentMatchDaySlot[]>(
            `${environment.apiUrl}/api/tournaments/${tournamentMatchDaySlot.tournamentId}/match-day-slots`,
            tournamentMatchDaySlot
        );
    }

    deleteTournamentMatchDaySlot(tournamentId: number, tournamentMatchDaySlotId: number) {
        return this.http.delete<void>(
            `${environment.apiUrl}/api/tournaments/${tournamentId}/match-day-slots/${tournamentMatchDaySlotId}`
        );
    }

    generateTournamentSchedule(tournamentId: number): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournaments/${tournamentId}/generate-schedule`, null);
    }

    createOrganisation(organisation: Organisation): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/organisations`, organisation);
    }

    updateOrganisation(organisation: Organisation): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/organisations`, organisation);
    }

    deleteOrganisation(organisationId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/api/organisations/${organisationId}`);
    }

    getOrganisation(organisationId: number): Observable<Organisation> {
        return this.http.get<Organisation>(`${environment.apiUrl}/api/organisations/${organisationId}`);
    }

    getOrganisations(): Observable<Organisation[]> {
        return this.http.get<Organisation[]>(`${environment.apiUrl}/api/organisations`);
    }

    getTournamentGroupStandings(tournamentGroupId: number): Observable<TournamentGroupStanding[]> {
        return this.http.get<TournamentGroupStanding[]>(`${environment.apiUrl}/api/tournament-groups/${tournamentGroupId}/standings`);
    }
}
