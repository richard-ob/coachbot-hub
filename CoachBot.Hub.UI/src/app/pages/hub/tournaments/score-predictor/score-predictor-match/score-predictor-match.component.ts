import { Component, Input, OnInit } from '@angular/core';
import { ScorePredictorService } from '@pages/hub/shared/services/score-predictor.service';
import { ScorePrediction } from '@pages/hub/shared/model/score-prediction.model';
import { TournamentGroupMatch } from '@pages/hub/shared/model/tournament-group-match.model';
import { TournamentPhase } from '@pages/hub/shared/model/tournament-phase.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'app-score-predictor-match',
    templateUrl: './score-predictor-match.component.html'
})
export class ScorePredictorMatchComponent implements OnInit {

    @Input() phaseMatch: TournamentGroupMatch;
    @Input() phase: TournamentPhase;
    @Input() scorePredictions: ScorePrediction[];
    scorePrediction: ScorePrediction = new ScorePrediction();
    isSubmitting = false;

    constructor(private scorePredictionService: ScorePredictorService, private snackBar: MatSnackBar) { }

    ngOnInit() {
        const existingPrediction = this.scorePredictions.find(p => p.matchId === this.phaseMatch.match.id);
        if (existingPrediction) {
            this.scorePrediction = existingPrediction;
        } else {
            this.scorePrediction.tournamentPhaseId = this.phase.id;
            this.scorePrediction.matchId = this.phaseMatch.id;
        }
    }

    submitPrediction() {
        this.isSubmitting = true;
        this.scorePredictionService.createScorePrediction(this.scorePrediction).subscribe(() => {
            this.isSubmitting = false;
            this.snackBar.open(
                `${this.phaseMatch.match.teamHome.teamCode} vs ${this.phaseMatch.match.teamAway.teamCode} Prediction submitted`,
                'Dismiss', { duration: 5000 }
            );
        });
    }

    incrementAwayGoals() {
        if (this.scorePrediction.awayGoalsPrediction < 40) {
            this.scorePrediction.awayGoalsPrediction++;
        }
    }

    incrementHomeGoals() {
        if (this.scorePrediction.homeGoalsPrediction < 40) {
            this.scorePrediction.homeGoalsPrediction++;
        }
    }

    decrementHomeGoals() {
        if (this.scorePrediction.homeGoalsPrediction > 0) {
            this.scorePrediction.homeGoalsPrediction--;
        }
    }

    decrementAwayGoals() {
        if (this.scorePrediction.awayGoalsPrediction > 0) {
            this.scorePrediction.awayGoalsPrediction--;
        }
    }
}
