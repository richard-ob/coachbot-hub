import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { PlayerProfileMatchesComponent } from './player-profile-matches/player-profile-matches.component';
import { PlayerProfileTournamentsComponent } from './player-profile-tournaments/player-profile-tournaments.component';
import { PlayerProfileStatisticsComponent } from './player-profile-statistics/player-profile-statistics.component';
import { PlayerTeamHistoryComponent } from './player-team-history/player-team-history.component';
import { PlayerProfileComponent } from './player-profile.component';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { CalendarHeatmap } from 'angular2-calendar-heatmap';
import { PlayerProfileRoutingModule } from './player-profile.routing-module';
import {
    PlayerProfileActivityHeatmapComponent
} from './player-profile-statistics/player-profile-activity-heatmap/player-profile-activity-heatmap.component';

@NgModule({
    declarations: [
        PlayerTeamHistoryComponent,
        PlayerProfileComponent,
        PlayerProfileMatchesComponent,
        PlayerProfileTournamentsComponent,
        PlayerProfileStatisticsComponent,
        PlayerProfileActivityHeatmapComponent,
        CalendarHeatmap
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        PlayerProfileRoutingModule
    ]
})
export class PlayerProfileModule { }