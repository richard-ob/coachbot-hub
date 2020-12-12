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
import { TeamsComponent } from './teams/teams.component';
import { TeamEditorListModule } from './team-editor-list/team-editor-list.module';
import { TeamEditorModule } from './team-editor/team-editor.module';
import { AssetImageUploaderModule } from '@shared/components/asset-image-uploader/asset-image-uploader.module';
import { ScorePredictorModule } from './tournaments/score-predictor/score-predictor.module';
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
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { FantasyTeamManagerModule } from './tournaments/fantasy/fantasy.module';
import { BotManagerModule } from './bot-manager/bot-manager.module';
import { TournamentManagerModule } from './tournaments/tournament-manager/tournament-manager.module';
import { TeamNameDisplayModule } from './shared/components/team-name-display/team-name-display.module';
import { MatchStatisticsModule } from './match-statistics/match-statistics.module';
import { MatchDataUploaderComponent } from './match-editor/match-data-uploader/match-data-uploader.component';
import { MatchTeamGoalsPipe } from './match-overview/helpers/match-team-goals.pipe';
import { PlayerOfTheMatchComponent } from './match-overview/components/player-of-the-match/player-of-the-match.component';
import { ServerRecoveryComponent } from './server-manager/server-recovery/server-recovery.component';
import { ServerCreatorComponent } from './server-manager/server-creator/server-creator.component';
import { TeamHeadToHeadModule } from './team-head-to-head/team-head-to-head.module';
import { GraphModule } from './shared/components/graphs/graph.module';

@NgModule({
  declarations: [
    MatchOverviewComponent,
    CountryNameConverterPipe,
    SteamIdToPlayerNamePipe,
    SecondsToMinutesPipe,
    ArrayFilterPipe,
    MatchStatisticPercentageCalculatorPipe,
    MatchStatisticTeamPercentageCalculatorPipe,
    MatchTeamGoalsPipe,
    ServerManagerComponent,
    ServerCreatorComponent,
    ServerRecoveryComponent,
    RegionManagerComponent,
    UpcomingMatchesComponent,
    SteamProfileFlagPipe,
    ProfileEditorComponent,
    SteamIDValidatorComponent,
    TournamentCreatorComponent,
    TournamentSeriesEditorComponent,
    OrganisationsComponent,
    OrganisationEditorComponent,
    MatchEditorComponent,
    MatchDataUploaderComponent,
    MatchOverviewLineupComponent,
    MatchOverviewPlayerStatisticsComponent,
    MatchOverviewLineupPositionComponent,
    PlayerOfTheMatchComponent,
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
    BsDropdownModule.forRoot(),
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
    TournamentManagerModule,
    TeamHeadToHeadModule,
    BotManagerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    BrowserAnimationsModule,
    NgPipesModule,
    SweetAlert2Module,
    HubPipesModule,
    ChromeColourPickerModule,
    AssetImageUploaderModule,
    ThSorterModule,
    TeamNameDisplayModule,
    MatchStatisticsModule,
    GraphModule,
    TimepickerModule.forRoot()
  ],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
  ]
})
export class HubModule { }
