import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ScorePredictorComponent } from './score-predictor.component';
import { ScorePredictorPlayerComponent } from './score-predictor-player/score-predictor-player.component';

const routes: Routes = [
    {
        path: 'score-predictor/:id',
        component: ScorePredictorComponent
    },
    {
        path: 'score-predictor/:tournamentEditionId/player/:playerId',
        component: ScorePredictorPlayerComponent
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class ScorePredictorRoutingModule { }
