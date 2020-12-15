import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';

@Component({
    selector: 'app-tournament-match-list',
    templateUrl: './tournament-match-list.component.html'
})
export class TournamentMatchListComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            console.log(this.tournamentId);
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

    editMatch(matchId: number) {
        this.router.navigate(['/match-editor/', matchId]);
    }

}
