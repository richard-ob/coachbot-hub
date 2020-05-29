import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';
import { ScorePrediction } from '@pages/hub/shared/model/score-prediction.model';
import { TournamentEdition } from '@pages/hub/shared/model/tournament-edition.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';

@Component({
    selector: 'app-score-predictor-player',
    templateUrl: './score-predictor-player.component.html'
})
export class ScorePredictorPlayerComponent implements OnInit {

    tournamentEdition: TournamentEdition;
    scorePredictions: ScorePrediction[];
    player: Player;
    isLoading = true;

    constructor(
        private scorePredictionService: ScorePredictorService,
        private tournamentService: TournamentService,
        private playerService: PlayerService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            const tournamentEditionId = +params.get('tournamentEditionId');
            const playerId = +params.get('playerId');
            this.tournamentService.getTournamentEdition(tournamentEditionId).subscribe(tournamentEdition => {
                this.tournamentEdition = tournamentEdition;
                this.scorePredictionService.getScorePredictionsForPlayer(tournamentEditionId, playerId).subscribe(predictions => {
                    this.scorePredictions = predictions;
                    this.playerService.getPlayer(playerId).subscribe(player => {
                        this.player = player;
                        this.isLoading = false;
                    });
                });
            });
        });
    }
}
