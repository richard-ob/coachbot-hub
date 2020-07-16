import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';

@Component({
    selector: 'app-score-predictor-hub-current-tournaments',
    templateUrl: './score-predictor-hub-current-tournaments.component.html'
})
export class ScorePredictorHubCurrentTournamentsComponent implements OnInit {

    tournaments: Tournament[];
    isLoading = true;

    constructor(private tournamentService: TournamentService, private router: Router) { }

    ngOnInit() {
        this.tournamentService.getCurrentTournaments().subscribe(tournaments => {
            this.tournaments = tournaments;
        });
    }

    navigateToTournament(tournamentId: number) {
        this.router.navigate(['/tournament/' + tournamentId + '/score-predictor']);
    }
}
