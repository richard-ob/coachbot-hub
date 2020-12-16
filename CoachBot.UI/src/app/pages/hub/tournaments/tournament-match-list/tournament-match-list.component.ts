import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';
import { TournamentStaffRole } from '@pages/hub/shared/model/tournament-staff-role.model';

@Component({
    selector: 'app-tournament-match-list',
    templateUrl: './tournament-match-list.component.html'
})
export class TournamentMatchListComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    currentPlayer: Player;
    accessDenied: boolean;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private playerService: PlayerService,
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
            this.playerService.getCurrentPlayer().subscribe((currentPlayer) => {
                if (tournament.tournamentStaff.some(staff => staff.playerId === currentPlayer.id && staff.role === TournamentStaffRole.Organiser)) {
                    this.tournament = tournament;
                } else {
                    this.accessDenied = true;
                }
                this.isLoading = false;
            });
        });
    }

    editMatch(matchId: number) {
        this.router.navigate(['/match-editor/', matchId]);
    }

}
