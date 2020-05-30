import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ScorePredictorComponent } from './score-predictor.component';
import { ScorePredictorPlayerComponent } from './score-predictor-player/score-predictor-player.component';

const routes: Routes = [
    {
        path: 'tournament/:id/score-predictor',
        component: ScorePredictorComponent
    },
    {
        path: 'tournament/:tournamentEditionId/score-predictor/player/:playerId',
        component: ScorePredictorPlayerComponent
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class ScorePredictorRoutingModule { }
