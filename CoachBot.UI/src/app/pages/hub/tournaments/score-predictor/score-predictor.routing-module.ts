import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ScorePredictorComponent } from './score-predictor.component';
import { ScorePredictorHubComponent } from './score-predictor-hub/score-predictor-hub.component';
import { ScorePredictorPlayerTournamentComponent } from './score-predictor-player-tournament/score-predictor-player-tournament.component';
import { ScorePredictorPlayerHistoryComponent } from './score-predictor-player-history/score-predictor-player-history.component';
import { AuthGuard } from '@core/guards/auth.guard';

const routes: Routes = [
    {
        path: 'score-predictor',
        component: ScorePredictorHubComponent,
        data: { title: 'Score Predictor' },
        canActivate: [AuthGuard]
    },
    {
        path: 'tournament/:id/score-predictor',
        component: ScorePredictorComponent,
        data: { title: 'Score Predictor' },
        canActivate: [AuthGuard]
    },
    {
        path: 'tournament/:tournamentId/score-predictor/player/:playerId',
        component: ScorePredictorPlayerTournamentComponent,
        data: { title: 'Score Predictor' },
        canActivate: [AuthGuard]
    },
    {
        path: 'score-predictor/player/:playerId',
        component: ScorePredictorPlayerHistoryComponent,
        data: { title: 'Score Predictor' },
        canActivate: [AuthGuard]
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class ScorePredictorRoutingModule { }
