import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentStaff } from '@pages/hub/shared/model/tournament-staff.model';
import { TournamentStaffRole } from '@pages/hub/shared/model/tournament-staff-role.model';
import { Player } from '@pages/hub/shared/model/player.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';

@Component({
    selector: 'app-tournament-staff-manager',
    templateUrl: './tournament-staff-manager.component.html'
})
export class TournamentStaffManagerComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    tournamentStaff = new TournamentStaff();
    tournamentRole = TournamentStaffRole;
    currentPlayer: Player;
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private playerService: PlayerService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.playerService.getCurrentPlayer().subscribe(currentPlayer => {
            this.currentPlayer = currentPlayer;
            this.route.parent.paramMap.pipe().subscribe(params => {
                this.tournamentId = +params.get('id');
                this.loadTournament();
            });
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

    deleteTournamentStaff(tournamentStaffId: number) {
        this.isLoading = true;
        this.tournamentService.deleteTournamentStaff(tournamentStaffId).subscribe(() => {
            this.isLoading = false;
            this.loadTournament();
        })
    }



}
