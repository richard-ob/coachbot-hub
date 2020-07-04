import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { TeamProfileRoutingModule } from './team-profile.routing-module';
import { TeamProfileMatchesComponent } from './team-profile-matches/team-profile-matches.component';
import { TeamProfilePlayersComponent } from './team-profile-players/team-profile-players.component';
import { TeamProfileStatisticsComponent } from './team-profile-statistics/team-profile-statistics.component';
import { TeamProfileTournamentsComponent } from './team-profile-tournaments/team-profile-tournaments.component';
import { TeamProfileComponent } from './team-profile.component';
import { RecentMatchesModule } from '../recent-matches/recent-matches.module';
import { TeamProfilePlayerHistoryComponent } from './team-profile-player-history/team-profile-player-history.component';
import {
    TeamProfileStatisticsLeaderboardComponent
} from './team-profile-statistics/team-profile-statistics-leaderboard/team-profile-statistics-leaderboard.component';
import { CalendarHeatmapModule } from 'src/app/shared/components/calendar-heatmap/calendar-heatmap.module';
import {
    TeamProfileActivityHeatmapComponent
} from './team-profile-statistics/team-profile-activity-heatmap/team-profile-activity-heatmap.component';
import { HubPipesModule } from '../shared/pipes/hub-pipes.module';
import { FormIndicatorModule } from '../shared/components/form-indictator/form-indicator.module';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { CoreModule } from '@core/core.module';
import { TeamPerformanceTrackerComponent } from './team-profile-statistics/team-performance-tracker/team-performance-tracker.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { TeamProfileSpotlightComponent } from './team-profile-statistics/team-profile-spotlight/team-profile-spotlight.component';

@NgModule({
    declarations: [
        TeamProfileMatchesComponent,
        TeamProfilePlayersComponent,
        TeamProfileStatisticsComponent,
        TeamProfileTournamentsComponent,
        TeamProfilePlayerHistoryComponent,
        TeamProfileStatisticsLeaderboardComponent,
        TeamProfileActivityHeatmapComponent,
        TeamPerformanceTrackerComponent,
        TeamProfileSpotlightComponent,
        TeamProfileComponent
    ],
    imports: [
        CommonModule,
        CoreModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        RecentMatchesModule,
        TeamProfileRoutingModule,
        CalendarHeatmapModule,
        HubPipesModule,
        FormIndicatorModule,
        NgxSkeletonLoaderModule,
        BsDropdownModule.forRoot(),
        NgxChartsModule
    ]
})
export class TeamProfileModule { }
