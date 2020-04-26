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
import { TeamListComponent } from './team-list/team-list.component';
import { SteamProfileFlagPipe } from './player-list/helpers/steam-profile-flag.pipe';
import { ProfileEditorComponent } from './profile-editor/profile-editor.component';
import { SteamIDValidatorComponent } from './profile-editor/steam-id-validator.component';
import { TeamProfileComponent } from './team-profile/team-profile.component';
import { UpcomingMatchesComponent } from './upcoming-matches/upcoming-matches.component';
import { TeamEditorComponent } from './team-editor/team-editor.component';
import { TeamEditorPlayerListComponent } from './team-editor/team-editor-player-list/team-editor-player-list.component';
import { TournamentCreatorComponent } from './tournaments/tournament-creator/tournament-creator.component';
import { TournamentEditorComponent } from './tournaments/tournament-editor/tournament-editor.component';
import { TournamentEditionManagerComponent } from './tournaments/tournament-edition-manager/tournament-edition-manager.component';
import {
  TournamentGroupTeamManagerComponent
} from './tournaments/tournament-edition-manager/tournament-group-team-manager/tournament-group-team-manager.component';
import { MatchEditorComponent } from './match-editor/match-editor.component';
import { DlDateTimeDateModule, DlDateTimePickerModule } from 'angular-bootstrap-datetimepicker';
import { PlayerProfileModule } from './player-profile/player-profile.module';
import { CurrentTournamentsComponent } from './tournaments/current-tournaments/current-tournaments.component';

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
    TeamListComponent,
    SteamProfileFlagPipe,
    ProfileEditorComponent,
    SteamIDValidatorComponent,
    TeamProfileComponent,
    TeamEditorComponent,
    TeamEditorPlayerListComponent,
    TournamentCreatorComponent,
    TournamentEditorComponent,
    TournamentEditionManagerComponent,
    TournamentGroupTeamManagerComponent,
    MatchEditorComponent,
    CurrentTournamentsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    SpinnerModule,
    NgxPaginationModule,
    DlDateTimeDateModule,
    DlDateTimePickerModule,
    PlayerProfileModule
  ]
})
export class HubModule { }
