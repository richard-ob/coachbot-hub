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
import { ScorePredictorMatchComponent } from './score-predictor-match/score-predictor-match.component';
import { ScorePredictorHubComponent } from './score-predictor-hub/score-predictor-hub.component';
import {
    ScorePredictorLeaderboardComponent
} from './score-predictor-hub/score-predictor-leaderboard/score-predictor-leaderboard.component';
import { ScorePredictorPlayerTournamentComponent } from './score-predictor-player-tournament/score-predictor-player-tournament.component';
import { ScorePredictorPlayerHistoryComponent } from './score-predictor-player-history/score-predictor-player-history.component';
import {
    ScorePredictorHubCurrentTournamentsComponent
} from './score-predictor-hub/score-predictor-hub-current-tournaments/score-predictor-hub-current-tournaments.component';
import { ScorePredictorSpotlightComponent } from './score-predictor-hub/score-predictor-spotlight/score-predictor-spotlight.component';
import { TeamNameDisplayModule } from '@pages/hub/shared/components/team-name-display/team-name-display.module';
import { CoreModule } from '@core/core.module';

@NgModule({
    declarations: [
        ScorePredictorComponent,
        ScorePredictorPlayerTournamentComponent,
        ScorePredictorMatchComponent,
        ScorePredictorHubComponent,
        ScorePredictorLeaderboardComponent,
        ScorePredictorPlayerHistoryComponent,
        ScorePredictorHubCurrentTournamentsComponent,
        ScorePredictorSpotlightComponent
    ],
    imports: [
        CoreModule,
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        NgPipesModule,
        NouisliderModule,
        HubPipesModule,
        MatSnackBarModule,
        TeamNameDisplayModule,
        ScorePredictorRoutingModule
    ]
})
export class ScorePredictorModule { }
