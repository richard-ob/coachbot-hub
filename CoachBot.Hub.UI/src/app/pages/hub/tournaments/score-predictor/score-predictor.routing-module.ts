import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ScorePredictorComponent } from './score-predictor.component';
import { ScorePredictorPlayerComponent } from './score-predictor-player/score-predictor-player.component';

const routes: Routes = [
    {
        path: 'tournament/:id/score-predictor',
        component: ScorePredictorComponent,
        data: { title: 'Score Predictor' }
    },
    {
        path: 'tournament/:tournamentId/score-predictor/player/:playerId',
        component: ScorePredictorPlayerComponent,
        data: { title: 'Score Predictor' }
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class ScorePredictorRoutingModule { }
