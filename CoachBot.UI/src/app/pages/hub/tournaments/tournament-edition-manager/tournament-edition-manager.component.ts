import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { Tournament } from '../../shared/model/tournament.model';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentGroup } from '../../shared/model/tournament-group.model';
import { TournamentStaff } from '../../shared/model/tournament-staff.model';
import { TournamentStaffRole } from '../../shared/model/tournament-staff-role.model';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';

@Component({
    selector: 'app-tournament-edition-manager',
    templateUrl: './tournament-edition-manager.component.html'
})
export class TournamentEditionManagerComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    tournamentGroup: TournamentGroup = new TournamentGroup();
    tournamentStaff = new TournamentStaff();
    tournamentRole = TournamentStaffRole;
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private fantasyService: FantasyService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.loadTournament();
        });
    }

    loadTournament() {
        this.isLoading = true;
        this.tournamentService.getTournament(this.tournamentId).subscribe(tournament => {
            this.tournament = tournament;
            this.isLoading = false;
        });
    }

    createTournamentStaff() {
        this.isSaving = true;
        this.tournamentStaff.tournamentId = this.tournamentId;
        this.tournamentService.createTournamentStaff(this.tournamentStaff).subscribe(() => {
            this.tournamentStaff = new TournamentStaff();
            this.isSaving = false;
            this.loadTournament();
        });
    }

    createTournamentGroup(tournamentStageId: number) {
        this.isLoading = true;
        this.tournamentGroup.tournamentStageId = tournamentStageId;
        this.tournamentService.createTournamentGroup(this.tournamentGroup).subscribe(() => {
            this.loadTournament();
            this.tournamentGroup = new TournamentGroup();
        });
    }

    deleteTournamentGroup(tournamentGroupId: number) {
        this.isLoading = true;
        this.tournamentService.deleteTournamentGroup(tournamentGroupId).subscribe(() => {
            this.loadTournament();
        });
    }

    removeTournamentGroupTeam(teamId: number, tournamentGroupId: number) {
        this.tournamentService.removeTournamentGroupTeam(teamId, tournamentGroupId).subscribe(() => {
            this.loadTournament();
        });
    }

    generateSchedule() {
        this.isLoading = true;
        this.tournamentService.generateTournamentSchedule(this.tournamentId).subscribe(() => {
            this.loadTournament();
        });
    }

    toggleIsPublic() {
        this.isLoading = true;
        this.tournament.isPublic = !this.tournament.isPublic;
        this.tournamentService.updateTournament(this.tournament).subscribe(() => {
            this.loadTournament();
        });
    }

    generateFantasyTeamSnapshots() {
        this.isLoading = true;
        this.fantasyService.generateFantasySnapshots(this.tournamentId).subscribe(() => {
            this.loadTournament();
        });
    }

    updateStartDate() {
        this.isLoading = true;
        this.tournamentService.updateTournament(this.tournament).subscribe(() => {
            this.loadTournament();
        });
    }

    editMatch(matchId: number) {
        this.router.navigate(['/match-editor/', matchId]);
    }

}
