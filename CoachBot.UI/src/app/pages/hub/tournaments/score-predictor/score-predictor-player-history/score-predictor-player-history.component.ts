import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';
import { ScorePrediction } from '@pages/hub/shared/model/score-prediction.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';

@Component({
    selector: 'app-score-predictor-player-history',
    templateUrl: './score-predictor-player-history.component.html'
})
export class ScorePredictorPlayerHistoryComponent implements OnInit {

    @Input() playerId: number;
    @Input() verticalPadding = true;
    @Input() verticalOverflow = false;
    @Input() useContainer = true;
    scorePredictions: ScorePrediction[];
    player: Player;
    isLoading = true;

    constructor(
        private scorePredictionService: ScorePredictorService,
        private playerService: PlayerService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        if (this.playerId) {
            this.loadPredictions(this.playerId);
        } else {
            this.route.paramMap.pipe().subscribe(params => {
                this.playerId = +params.get('playerId');
                this.loadPredictions(this.playerId);
            });
        }
    }

    loadPredictions(playerId) {
        this.scorePredictionService.getHistoricScorePredictionsForPlayer(playerId).subscribe(predictions => {
            this.scorePredictions = predictions;
            this.playerService.getPlayer(playerId).subscribe(player => {
                this.player = player;
                this.isLoading = false;
            });
        });
    }
}
