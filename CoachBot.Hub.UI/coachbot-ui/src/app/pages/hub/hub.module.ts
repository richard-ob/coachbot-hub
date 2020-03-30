import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatchOverviewComponent } from './match-overview/match-overview.component';
import { CountryNameConverterPipe } from './match-overview/helpers/country-name-converter.pipe';
import { SteamIdToPlayerNamePipe } from './match-overview/helpers/steamid-to-player-name.pipe';
import { SecondsToMinutesPipe } from './match-overview/helpers/seconds-to-minute.pipe';
import { ArrayFilterPipe } from './match-overview/helpers/array-filter.pipe';
import { MatchStatisticPercentageCalculatorPipe } from './match-overview/helpers/match-statistic-percentage-calculator.pipe';
import { MatchStatisticTeamPercentageCalculatorPipe } from './match-overview/helpers/match-statistic-team-percentage-calculator.pipe';
import { HorizontalBarGraphComponent } from './match-overview/components/horizontal-bar-graph/horizontal-bar-graph.component';
import { PercentageSharePipe } from './match-overview/helpers/percentage-share.pipe';
import { CircleGraphComponent } from './match-overview/components/circle-graph/circle-graph.component';
import { ServerManagerComponent } from './server-manager/server-manager.component';
import { RegionManagerComponent } from './region-manager/region-manager.component';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RecentMatchesComponent } from './recent-matches/recent-matches.component';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { PlayerListComponent } from './player-list/player-list.component';
import { PlayerProfileComponent } from './player-profile/player-profile.component';
import { TeamListComponent } from './team-list/team-list.component';
import { SteamProfileFlagPipe } from './player-list/helpers/steam-profile-flag.pipe';
import { ProfileEditorComponent } from './profile-editor/profile-editor.component';
import { SteamIDValidatorComponent } from './profile-editor/steam-id-validator.component';
import { TeamProfileComponent } from './team-profile/team-profile.component';
import { UpcomingMatchesComponent } from './upcoming-matches/upcoming-matches.component';
import { TeamEditorComponent } from './team-editor/team-editor.component';
import { TeamEditorPlayerListComponent } from './team-editor/team-editor-player-list/team-editor-player-list.component';

@NgModule({
  declarations: [
    MatchOverviewComponent,
    CountryNameConverterPipe,
    SteamIdToPlayerNamePipe,
    SecondsToMinutesPipe,
    ArrayFilterPipe,
    MatchStatisticPercentageCalculatorPipe,
    MatchStatisticTeamPercentageCalculatorPipe,
    HorizontalBarGraphComponent,
    CircleGraphComponent,
    PercentageSharePipe,
    ServerManagerComponent,
    RegionManagerComponent,
    RecentMatchesComponent,
    UpcomingMatchesComponent,
    PlayerListComponent,
    PlayerProfileComponent,
    TeamListComponent,
    SteamProfileFlagPipe,
    ProfileEditorComponent,
    SteamIDValidatorComponent,
    TeamProfileComponent,
    TeamEditorComponent,
    TeamEditorPlayerListComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    SpinnerModule,
    NgxPaginationModule
  ]
})
export class HubModule { }
