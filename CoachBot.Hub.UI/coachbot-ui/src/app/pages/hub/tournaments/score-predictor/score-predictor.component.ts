import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TournamentEdition } from '@pages/hub/shared/model/tournament-edition.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';
import { ScorePrediction } from '@pages/hub/shared/model/score-prediction.model';
import { TournamentPhase } from '@pages/hub/shared/model/tournament-phase.model';
import { ScorePredictionLeaderboardPlayer } from '@pages/hub/shared/model/score-prediction-leaderboard-player.model';

@Component({
    selector: 'app-score-predictor',
    templateUrl: './score-predictor.component.html'
})
export class ScorePredictorComponent implements OnInit {

    tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
    phase: TournamentPhase;
    leaderboard: ScorePredictionLeaderboardPlayer[];
    scorePredictions: ScorePrediction[];
    newPrediction = new ScorePrediction();
    isLoading = true;

    constructor(
        private scorePredictionService: ScorePredictorService,
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentEditionId = +params.get('id');
            this.tournamentService.getCurrentPhase(this.tournamentEditionId).subscribe(phase => {
                this.phase = phase;
                this.phase.tournamentGroupMatches =
                    this.phase.tournamentGroupMatches.filter(m => new Date(m.match.scheduledKickOff) > new Date());
                this.scorePredictionService.getScorePredictions(this.tournamentEditionId).subscribe(scorePredictions => {
                    this.scorePredictions = scorePredictions;
                    this.scorePredictionService.getScorePredictionLeaderboard(this.tournamentEditionId).subscribe(leaderboard => {
                        this.leaderboard = leaderboard;
                        this.isLoading = false;
                    });
                });
            });
        });
    }
}
