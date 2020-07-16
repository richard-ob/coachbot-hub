import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ScorePredictionLeaderboardPlayer } from '@pages/hub/shared/model/score-prediction-leaderboard-player.model';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';

@Component({
    selector: 'app-score-predictor-spotlight',
    templateUrl: './score-predictor-spotlight.component.html'
})
export class ScorePredictorSpotlightComponent implements OnInit {

    spotlightPlayer: ScorePredictionLeaderboardPlayer;
    isLoading = true;

    constructor(private scorePredictorService: ScorePredictorService, private router: Router) { }

    ngOnInit() {
        this.scorePredictorService.getScorePredictionMonthLeader().subscribe(leader => {
            this.spotlightPlayer = leader;
            this.isLoading = false;
        });
    }

    navigateToPlayerHistory() {
        this.router.navigate(['/score-predictor/player/', this.spotlightPlayer.playerId]);
    }

}
