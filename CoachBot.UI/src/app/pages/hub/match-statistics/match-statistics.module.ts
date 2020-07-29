import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { CoreModule } from '@core/core.module';
import { UnlinkedMatchStatisticsComponent } from './unlinked-match-statistics/unlinked-match-statistics.component';
import { MatchStatisticsRoutingModule } from './match-statistics.routing-module';
import { TeamNameDisplayModule } from '../shared/components/team-name-display/team-name-display.module';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { MatchDataTokenPipe } from './unlinked-match-statistics/match-data-token.pipe';

@NgModule({
    declarations: [
        UnlinkedMatchStatisticsComponent,
        MatchDataTokenPipe
    ],
    imports: [
        CommonModule,
        CoreModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        MatchStatisticsRoutingModule,
        TeamNameDisplayModule,
        NgxJsonViewerModule,
        SweetAlert2Module
    ]
})
export class MatchStatisticsModule { }
