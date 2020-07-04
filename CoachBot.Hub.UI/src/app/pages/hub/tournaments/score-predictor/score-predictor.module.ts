import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { NgPipesModule } from 'ngx-pipes';
import { NouisliderModule } from 'ng2-nouislider';
import { HubPipesModule } from '@pages/hub/shared/pipes/hub-pipes.module';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ScorePredictorComponent } from './score-predictor.component';
import { ScorePredictorRoutingModule } from './score-predictor.routing-module';
import { ScorePredictorPlayerComponent } from './score-predictor-player/score-predictor-player.component';
import { ScorePredictorMatchComponent } from './score-predictor-match/score-predictor-match.component';

@NgModule({
    declarations: [
        ScorePredictorComponent,
        ScorePredictorPlayerComponent,
        ScorePredictorMatchComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        NgPipesModule,
        NouisliderModule,
        HubPipesModule,
        MatSnackBarModule,
        ScorePredictorRoutingModule
    ]
})
export class ScorePredictorModule { }
