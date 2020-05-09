import { Component, OnInit } from '@angular/core';
import { Tournament } from '../../shared/tournament.model';
import { TournamentService } from '../../shared/services/tournament.service';
import { TournamentEdition } from '../../shared/model/tournament-edition.model';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentGroup } from '../../shared/model/tournament-group.model';
import { TournamentEditionStaff } from '../../shared/model/tournament-edition-staff.model';
import { TournamentStaffRole } from '../../shared/model/tournament-staff-role.model';

@Component({
    selector: 'app-tournament-edition-manager',
    templateUrl: './tournament-edition-manager.component.html'
})
export class TournamentEditionManagerComponent implements OnInit {

    tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
    tournamentGroup: TournamentGroup = new TournamentGroup();
    tournamentStaff = new TournamentEditionStaff();
    tournamentEditionRole = TournamentStaffRole;
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentEditionId = +params.get('id');
            this.loadTournamentEdition();
        });
    }

    loadTournamentEdition() {
        this.isLoading = true;
        this.tournamentService.getTournamentEdition(this.tournamentEditionId).subscribe(tournamentEdition => {
            this.tournamentEdition = tournamentEdition;
            this.isLoading = false;
        });
    }

    createTournamentEditionStaff() {
        this.isSaving = true;
        this.tournamentStaff.tournamentEditionId = this.tournamentEditionId;
        this.tournamentService.createTournamentEditionStaff(this.tournamentStaff).subscribe(() => {
            this.tournamentStaff = new TournamentEditionStaff();
            this.isSaving = false;
            this.loadTournamentEdition();
        });
    }

    createTournamentGroup(tournamentStageId: number) {
        this.isLoading = true;
        this.tournamentGroup.tournamentStageId = tournamentStageId;
        this.tournamentService.createTournamentGroup(this.tournamentGroup).subscribe(() => {
            this.loadTournamentEdition();
            this.tournamentGroup = new TournamentGroup();
        });
    }

    deleteTournamentGroup(tournamentGroupId: number) {
        this.isLoading = true;
        this.tournamentService.deleteTournamentGroup(tournamentGroupId).subscribe(() => {
            this.loadTournamentEdition();
        });
    }

    removeTournamentGroupTeam(teamId: number, tournamentGroupId: number) {
        this.tournamentService.removeTournamentGroupTeam(teamId, tournamentGroupId).subscribe(() => {
            this.loadTournamentEdition();
        });
    }

    generateSchedule() {
        this.isLoading = true;
        this.tournamentService.generateTournamentSchedule(this.tournamentEditionId).subscribe(() => {
            this.loadTournamentEdition();
        });
    }

    toggleIsPublic() {
        this.isLoading = true;
        this.tournamentEdition.isPublic = !this.tournamentEdition.isPublic;
        this.tournamentService.updateTournamentEdition(this.tournamentEdition).subscribe(() => {
            this.loadTournamentEdition();
        });
    }

    editMatch(matchId: number) {
        this.router.navigate(['/match-editor/', matchId]);
    }

}
