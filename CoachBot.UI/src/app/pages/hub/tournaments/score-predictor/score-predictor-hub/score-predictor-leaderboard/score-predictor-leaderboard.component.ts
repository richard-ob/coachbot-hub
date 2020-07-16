import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';
import { ScorePredictionLeaderboardPlayer } from '@pages/hub/shared/model/score-prediction-leaderboard-player.model';

@Component({
    selector: 'app-score-predictor-leaderboard',
    templateUrl: './score-predictor-leaderboard.component.html'
})
export class ScorePredictorLeaderboardComponent implements OnInit {

    tournament: Tournament;
    leaderboard: ScorePredictionLeaderboardPlayer[];
    player: Player;
    isLoading = true;

    constructor(
        private scorePredictionService: ScorePredictorService,
        private playerService: PlayerService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.scorePredictionService.getScorePredictionsGlobalLeaderboard().subscribe(leaderboard => {
            this.leaderboard = leaderboard;
            // this.playerService.getPlayer(playerId).subscribe(player => {
            //    this.player = player;
            this.isLoading = false;
            // });
        });
    }
}
