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
import { TeamListComponent } from './team-list/team-list.component';
import { SteamProfileFlagPipe } from './player-list/helpers/steam-profile-flag.pipe';
import { ProfileEditorComponent } from './profile-editor/profile-editor.component';
import { SteamIDValidatorComponent } from './profile-editor/steam-id-validator.component';
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
import { AssetImageUploaderComponent } from 'src/app/shared/components/asset-image-uploader/asset-image-uploader.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';
import { NgPipesModule } from 'ngx-pipes';
import { TeamProfileModule } from './team-profile/team-profile.module';
import { RecentMatchesModule } from './recent-matches/recent-matches.module';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import {
  TeamEditorDiscordIntegrationComponent
} from './team-editor/team-editor-discord-integration/team-editor-discord-integration.component';
import {
  DiscordChannelEditorComponent
} from './team-editor/team-editor-discord-integration/discord-channel-editor/discord-channel-editor.component';
import {
  DiscordGuildEditorComponent
} from './team-editor/team-editor-discord-integration/discord-guild-editor/discord-guild-editor.component';
import { ChromeColourPickerModule } from 'src/app/shared/components/chrome-colour-picker/chrome-colour-picker.module';
import {
  DiscordEmoteDisplayNamePipe
} from './team-editor/team-editor-discord-integration/discord-guild-editor/discord-emote-display-name.pipe';
import { TournamentOverviewModule } from './tournaments/tournament-overview/tournament-overview.module';
import { TeamListModule } from './team-list/team-list.module';
import { PlayerListModule } from './player-list/player-list.module';
import { HubPipesModule } from './shared/pipes/hub-pipes.module';

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
    TeamEditorComponent,
    TeamEditorPlayerListComponent,
    TournamentCreatorComponent,
    TournamentEditorComponent,
    TournamentEditionManagerComponent,
    TournamentGroupTeamManagerComponent,
    MatchEditorComponent,
    CurrentTournamentsComponent,
    AssetImageUploaderComponent,
    TeamEditorDiscordIntegrationComponent,
    DiscordChannelEditorComponent,
    DiscordGuildEditorComponent,
    DiscordEmoteDisplayNamePipe
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
    RecentMatchesModule,
    MatDatepickerModule,
    MatNativeDateModule,
    BrowserAnimationsModule,
    NgPipesModule,
    SweetAlert2Module,
    HubPipesModule,
    ChromeColourPickerModule
  ],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
  ]
})
export class HubModule { }
