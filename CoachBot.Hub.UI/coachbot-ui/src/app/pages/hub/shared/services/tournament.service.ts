
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { TournamentGroupTeamDto } from '../model/dtos/tournament-group-team-dto.model';
import { TournamentEdition } from '../model/tournament-edition.model';
import { Tournament } from '../tournament.model';
import { TournamentGroup } from '../model/tournament-group.model';
import { TournamentEditionStaff } from '../model/tournament-edition-staff.model';
import { Team } from '../model/team.model';
import { TournamentPhase } from '../model/tournament-phase.model';
import { TournamentEditionMatchDaySlot } from '../model/tournament-edition-match-day-slot.model';

@Injectable({
    providedIn: 'root'
})
export class TournamentService {

    constructor(private http: HttpClient) { }

    getTournaments(): Observable<Tournament[]> {
        return this.http.get<Tournament[]>(`${environment.apiUrl}/api/tournaments`);
    }

    getTournament(tournamentId: number): Observable<Tournament> {
        return this.http.get<Tournament>(`${environment.apiUrl}/api/tournaments/${tournamentId}`);
    }

    getTournamentEditions(): Observable<TournamentEdition[]> {
        return this.http.get<TournamentEdition[]>(`${environment.apiUrl}/api/tournament-editions`);
    }

    getTournamentEdition(tournamentEditionId: number): Observable<TournamentEdition> {
        return this.http.get<TournamentEdition>(`${environment.apiUrl}/api/tournament-editions/${tournamentEditionId}`);
    }

    createTournament(tournament: Tournament): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournaments`, tournament);
    }

    createTournamentEdition(tournamentEdition: TournamentEdition): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournament-editions`, tournamentEdition);
    }

    updateTournamentEdition(tournamentEdition: TournamentEdition): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/tournament-editions`, tournamentEdition);
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

    getTournamentEditionStaff(tournamentEditionId: number): Observable<TournamentEditionStaff[]> {
        return this.http.get<TournamentEditionStaff[]>(`${environment.apiUrl}/api/tournament-editions/${tournamentEditionId}/staff`);
    }

    createTournamentEditionStaff(tournamentEditionStaff: TournamentEditionStaff): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournament-edition-staff`, tournamentEditionStaff);
    }

    updateTournamentEditionStaff(tournamentEditionStaff: TournamentEditionStaff): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/tournament-edition-staff`, tournamentEditionStaff);
    }

    deleteTournamentEditionStaff(tournamentEditionStaffId: number): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/api/tournament-edition-staff/${tournamentEditionStaffId}`);
    }

    getTournamentTeams(tournamentEditionId: number): Observable<Team[]> {
        return this.http.get<Team[]>(`${environment.apiUrl}/api/tournament-editions/${tournamentEditionId}/teams`);
    }

    getCurrentPhase(tournamentEditionId: number): Observable<TournamentPhase> {
        return this.http.get<TournamentPhase>(`${environment.apiUrl}/api/tournament-editions/${tournamentEditionId}/current-phase`);
    }

    getTournamentEditionMatchDaySlots(tournamentEditionId: number): Observable<TournamentEditionMatchDaySlot[]> {
        return this.http.get<TournamentEditionMatchDaySlot[]>(
            `${environment.apiUrl}/api/tournament-editions/${tournamentEditionId}/match-day-slots`
        );
    }

    createTournamentEditionMatchDaySlot(tournamentEditionMatchDaySlot: TournamentEditionMatchDaySlot) {
        return this.http.post<TournamentEditionMatchDaySlot[]>(
            `${environment.apiUrl}/api/tournament-editions/${tournamentEditionMatchDaySlot.tournamentEditionId}/match-day-slots`,
            tournamentEditionMatchDaySlot
        );
    }

    deleteTournamentEditionMatchDaySlot(tournamentEditionId: number, tournamentEditionMatchDaySlotId: number) {
        return this.http.delete<void>(
            `${environment.apiUrl}/api/tournament-editions/${tournamentEditionId}/match-day-slots/${tournamentEditionMatchDaySlotId}`
        );
    }

    generateTournamentSchedule(tournamentEditionId: number): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/tournament-editions/${tournamentEditionId}/generate-schedule`, null);
    }
}
