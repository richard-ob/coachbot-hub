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
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { SteamProfileFlagPipe } from './player-list/helpers/steam-profile-flag.pipe';
import { ProfileEditorComponent } from './profile-editor/profile-editor.component';
import { SteamIDValidatorComponent } from './profile-editor/steam-id-validator.component';
import { UpcomingMatchesComponent } from './upcoming-matches/upcoming-matches.component';
import { TournamentCreatorComponent } from './tournaments/tournament-creator/tournament-creator.component';
import { TournamentEditionManagerComponent } from './tournaments/tournament-edition-manager/tournament-edition-manager.component';
import {
  TournamentGroupTeamManagerComponent
} from './tournaments/tournament-edition-manager/tournament-group-team-manager/tournament-group-team-manager.component';
import { MatchEditorComponent } from './match-editor/match-editor.component';
import { DlDateTimeDateModule, DlDateTimePickerModule } from 'angular-bootstrap-datetimepicker';
import { PlayerProfileModule } from './player-profile/player-profile.module';
import { CurrentTournamentsComponent } from './tournaments/current-tournaments/current-tournaments.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';
import { NgPipesModule } from 'ngx-pipes';
import { TeamProfileModule } from './team-profile/team-profile.module';
import { RecentMatchesModule } from './recent-matches/recent-matches.module';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { ChromeColourPickerModule } from 'src/app/shared/components/chrome-colour-picker/chrome-colour-picker.module';
import { TournamentOverviewModule } from './tournaments/tournament-overview/tournament-overview.module';
import { TeamListModule } from './team-list/team-list.module';
import { PlayerListModule } from './player-list/player-list.module';
import { HubPipesModule } from './shared/pipes/hub-pipes.module';
import { FantasyTeamManagerModule } from './tournaments/fantasy-team-manager/fantasy-team-manager.module';
import { TeamsComponent } from './teams/teams.component';
import { TeamEditorListModule } from './team-editor-list/team-editor-list.module';
import { TeamEditorModule } from './team-editor/team-editor.module';
import { AssetImageUploaderModule } from '@shared/components/asset-image-uploader/asset-image-uploader.module';
import { ScorePredictorModule } from './tournaments/score-predictor/score-predictor.module';
import {
  TournamentMatchDaySlotManagerComponent
} from './tournaments/tournament-edition-manager/tournament-match-day-slot-manager/tournament-match-day-slot-manager.component';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { MatchOverviewLineupComponent } from './match-overview/components/match-overview-lineup/match-overview-lineup.component';
import {
  MatchOverviewPlayerStatisticsComponent
} from './match-overview/components/match-overview-player-statistics/match-overview-player-statistics.component';
import { ThSorterModule } from '@shared/components/th-sorter/th-sort.module';
import {
  MatchOverviewLineupPositionComponent
} from './match-overview/components/match-overview-lineup/match-overview-lineup-position/match-overview-lineup-position.component';
import { TournamentSeriesEditorComponent } from './tournaments/tournament-series-editor/tournament-series-editor.component';
import { OrganisationsComponent } from './tournaments/organisations/organisations.component';
import { OrganisationEditorComponent } from './tournaments/organisations/organisation-editor/organisation-editor.component';
import { PreviousTournamentsComponent } from './tournaments/previous-tournaments/previous-tournaments.component';

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
    UpcomingMatchesComponent,
    SteamProfileFlagPipe,
    ProfileEditorComponent,
    SteamIDValidatorComponent,
    TournamentCreatorComponent,
    TournamentSeriesEditorComponent,
    TournamentEditionManagerComponent,
    TournamentGroupTeamManagerComponent,
    TournamentMatchDaySlotManagerComponent,
    OrganisationsComponent,
    OrganisationEditorComponent,
    MatchEditorComponent,
    MatchOverviewLineupComponent,
    MatchOverviewPlayerStatisticsComponent,
    MatchOverviewLineupPositionComponent,
    CurrentTournamentsComponent,
    PreviousTournamentsComponent,
    TeamsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    SpinnerModule,
    NgxPaginationModule,
    DlDateTimeDateModule,
    DlDateTimePickerModule,
    PlayerProfileModule,
    TeamProfileModule,
    TeamListModule,
    PlayerListModule,
    TournamentOverviewModule,
    FantasyTeamManagerModule,
    ScorePredictorModule,
    RecentMatchesModule,
    TeamEditorListModule,
    TeamEditorModule,
    MatDatepickerModule,
    MatNativeDateModule,
    BrowserAnimationsModule,
    NgPipesModule,
    SweetAlert2Module,
    HubPipesModule,
    ChromeColourPickerModule,
    AssetImageUploaderModule,
    ThSorterModule,
    TimepickerModule.forRoot()
  ],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
  ]
})
export class HubModule { }
