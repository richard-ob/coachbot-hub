import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormIndicatorModule } from '../shared/components/form-indictator/form-indicator.module';
import { ThSorterModule } from '@shared/components/th-sorter/th-sort.module';
import { NgPipesModule } from 'ngx-pipes';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { HubPipesModule } from '../shared/pipes/hub-pipes.module';
import { TeamHeadToHeadComponent } from './team-head-to-head.component';
import { TeamHeadToHeadRoutingModule } from './team-head-to-head.routing-module';
import { TeamHeadToHeadSelectorComponent } from './team-head-to-head-selector/team-head-to-head-selector.component';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { RecentMatchesModule } from '../recent-matches/recent-matches.module';
import { GraphModule } from '../shared/components/graphs/graph.module';
import { TeamHeadToHeadResultsComponent } from './team-head-to-head-results/team-head-to-head-results.component';
import { TeamNameDisplayModule } from '../shared/components/team-name-display/team-name-display.module';
import { DynamicResultBoxesComponent } from './team-head-to-head-results/dynamic-result-boxes/dynamic-result-boxes.component';
import { StatisticsLeaderboardModule } from '../shared/components/statistics-leaderboard/statistics-leaderboard.module';
import { TeamHeadToHeadSpotlightComponent } from './team-head-to-head-spotlight/team-head-to-head-spotlight.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@NgModule({
    declarations: [
        TeamHeadToHeadComponent,
        TeamHeadToHeadSelectorComponent,
        TeamHeadToHeadResultsComponent,
        DynamicResultBoxesComponent,
        TeamHeadToHeadSpotlightComponent
    ],
    exports: [
        TeamHeadToHeadComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        FormIndicatorModule,
        ThSorterModule,
        NgPipesModule,
        SweetAlert2Module,
        HubPipesModule,
        NgScrollbarModule,
        RecentMatchesModule,
        RecentMatchesModule,
        GraphModule,
        TeamNameDisplayModule,
        StatisticsLeaderboardModule,
        TeamHeadToHeadRoutingModule,
        BsDropdownModule
    ]
})
export class TeamHeadToHeadModule { }
