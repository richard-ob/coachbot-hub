import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';
import { ScorePrediction } from '@pages/hub/shared/model/score-prediction.model';
import { TournamentPhase } from '@pages/hub/shared/model/tournament-phase.model';
import { ScorePredictionLeaderboardPlayer } from '@pages/hub/shared/model/score-prediction-leaderboard-player.model';
import { Tournament } from '@pages/hub/shared/model/tournament.model';

@Component({
    selector: 'app-score-predictor',
    templateUrl: './score-predictor.component.html'
})
export class ScorePredictorComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    phase: TournamentPhase;
    leaderboard: ScorePredictionLeaderboardPlayer[];
    scorePredictions: ScorePrediction[];
    newPrediction = new ScorePrediction();
    isLoading = true;

    constructor(
        private scorePredictionService: ScorePredictorService,
        private tournamentService: TournamentService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.tournamentService.getCurrentPhase(this.tournamentId).subscribe(phase => {
                this.phase = phase;
                if (phase) {
                    this.phase.tournamentGroupMatches =
                        this.phase.tournamentGroupMatches.filter(m =>
                            new Date(m.match.kickOff) > new Date() && m.match.teamHome != null && m.match.teamAway != null
                            && m.match.matchStatisticsId == null
                        );
                }
                this.scorePredictionService.getScorePredictions(this.tournamentId).subscribe(scorePredictions => {
                    this.scorePredictions = scorePredictions;
                    this.scorePredictionService.getScorePredictionLeaderboard(this.tournamentId).subscribe(leaderboard => {
                        this.leaderboard = leaderboard;
                        this.isLoading = false;
                    });
                });
            });
        });
    }
}
