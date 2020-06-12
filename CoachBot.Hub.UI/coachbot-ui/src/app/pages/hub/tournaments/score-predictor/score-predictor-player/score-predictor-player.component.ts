import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';
import { ScorePrediction } from '@pages/hub/shared/model/score-prediction.model';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';

@Component({
    selector: 'app-score-predictor-player',
    templateUrl: './score-predictor-player.component.html'
})
export class ScorePredictorPlayerComponent implements OnInit {

    tournament: Tournament;
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
            const tournamentId = +params.get('tournamentId');
            const playerId = +params.get('playerId');
            this.tournamentService.getTournament(tournamentId).subscribe(tournament => {
                this.tournament = tournament;
                this.scorePredictionService.getScorePredictionsForPlayer(tournamentId, playerId).subscribe(predictions => {
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
