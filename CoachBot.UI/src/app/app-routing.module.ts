import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HeroComponent } from './pages/hero/hero.component';
import { MatchOverviewComponent } from './pages/hub/match-overview/match-overview.component';
import { ServerManagerComponent } from './pages/hub/server-manager/server-manager.component';
import { LoginComponent } from './pages/login/login.component';
import { ErrorComponent } from './pages/error/error.component';
import { RegionManagerComponent } from './pages/hub/region-manager/region-manager.component';
import { RecentMatchesComponent } from './pages/hub/recent-matches/recent-matches.component';
import { PlayerListComponent } from './pages/hub/player-list/player-list.component';
import { TeamListComponent } from './pages/hub/team-list/team-list.component';
import { ProfileEditorComponent } from './pages/hub/profile-editor/profile-editor.component';
import { SteamIDValidatorComponent } from './pages/hub/profile-editor/steam-id-validator.component';
import { ManualComponent } from './pages/manual/manual.component';
import { TeamComponent } from './pages/team/team.component';
import { TournamentCreatorComponent } from './pages/hub/tournaments/tournament-creator/tournament-creator.component';
import { MatchEditorComponent } from './pages/hub/match-editor/match-editor.component';
import { UpcomingMatchesComponent } from './pages/hub/upcoming-matches/upcoming-matches.component';
import { CurrentTournamentsComponent } from './pages/hub/tournaments/current-tournaments/current-tournaments.component';
import { MatchmakingComponent } from './pages/matchmaking/matchmaking.component';
import { AboutComponent } from '@pages/about/about.component';
import { ServerManualComponent } from '@pages/server-manual/server-manual.component';
import { NewsComponent } from '@pages/news/news.component';
import { TeamsComponent } from '@pages/hub/teams/teams.component';
import { TeamEditorListComponent } from '@pages/hub/team-editor-list/team-editor-list.component';
import { TournamentSeriesEditorComponent } from '@pages/hub/tournaments/tournament-series-editor/tournament-series-editor.component';
import { OrganisationsComponent } from '@pages/hub/tournaments/organisations/organisations.component';
import { OrganisationEditorComponent } from '@pages/hub/tournaments/organisations/organisation-editor/organisation-editor.component';
import { PreviousTournamentsComponent } from '@pages/hub/tournaments/previous-tournaments/previous-tournaments.component';
import { MediaComponent } from '@pages/media/media.component';
import { TroubleshootingComponent } from '@pages/troubleshooting/troubleshooting.component';
import { BotManualComponent } from '@pages/bot-manual/bot-manual.component';
import { AuthGuard } from '@core/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: HeroComponent
  },
  {
    path: 'manual',
    component: ManualComponent,
    data: { title:  $localize`:@@globals.manual:Manual` }
  },
  {
    path: 'server-manual',
    component: ServerManualComponent,
    data: { title: $localize`:@@globals.serverManual:Server Manual` }
  },
  {
    path: 'bot-manual',
    component: BotManualComponent,
    data: { title: $localize`:@@globals.botManual:Bot Manual` }
  },
  {
    path: 'troubleshooting',
    component: TroubleshootingComponent,
    data: { title: $localize`:@@globals.troubleshooting:Troubleshooting` }
  },
  {
    path: 'news',
    component: NewsComponent,
    data: { title: $localize`:@@globals.news:News` }
  },
  {
    path: 'media',
    component: MediaComponent,
    data: { title: $localize`:@@globals.media:Media` }
  },
  {
    path: 'team',
    component: TeamComponent,
    data: { title: $localize`:@@globals.team:Team` }
  },
  {
    path: 'about',
    component: AboutComponent,
    data: { title: $localize`:@@globals.about:About` }
  },
  {
    path: 'match-overview/:id',
    component: MatchOverviewComponent,
    data: { title: $localize`:@@globals.matchOverview:Match Overview` }
  },
  {
    path: 'server-manager',
    component: ServerManagerComponent,
    data: { title: $localize`:@@globals.serverManager:Server Manager` },
    canActivate: [AuthGuard]
  },
  {
    path: 'region-manager',
    component: RegionManagerComponent,
    data: { title: $localize`:@@globals.regionManager:Region Manager` },
    canActivate: [AuthGuard]
  },
  {
    path: 'recent-matches',
    component: RecentMatchesComponent,
    data: { title: $localize`:@@globals.results:Results` }
  },
  {
    path: 'upcoming-matches',
    component: UpcomingMatchesComponent,
    data: { title: $localize`:@@globals.fixtures:Fixtures` }
  },
  {
    path: 'player-list',
    component: PlayerListComponent,
    data: { title: $localize`:@@globals.playerStatistics:Player Statistics` }
  },
  {
    path: 'team-list',
    component: TeamListComponent,
    data: { title: $localize`:@@globals.teamStatistics:Team Statistics` }
  },
  {
    path: 'edit-profile',
    component: ProfileEditorComponent,
    data: { title: $localize`:@@globals.editProfile:Edit Profile` },
    canActivate: [AuthGuard]
  },
  {
    path: 'team-editor-list',
    component: TeamEditorListComponent,
    data: { title: $localize`:@@globals.manageTeams:Manage Teams` },
    canActivate: [AuthGuard]
  },
  {
    path: 'teams',
    component: TeamsComponent,
    data: { title: $localize`:@@globals.teams:Teams` }
  },
  {
    path: 'validate-steamid',
    component: SteamIDValidatorComponent
  },
  {
    path: 'tournament-creator',
    component: TournamentCreatorComponent,
    data: { title: $localize`:@@globals.tournamentCreator:Tournament Creator` },
    canActivate: [AuthGuard]
  },
  {
    path: 'tournament-editor/:id',
    component: TournamentSeriesEditorComponent,
    data: { title: $localize`:@@globals.tournament:Tournament` }
  },
  {
    path: 'tournaments',
    component: CurrentTournamentsComponent,
    data: { title: $localize`:@@globals.tournaments:Tournaments` }
  },
  {
    path: 'tournament-history',
    component: PreviousTournamentsComponent,
    data: { title: $localize`:@@globals.tournamentHistory:Tournament History` }
  },
  {
    path: 'organisations',
    component: OrganisationsComponent,
    data: { title: $localize`:@@globals.organisations:Organisations` },
    canActivate: [AuthGuard]
  },
  {
    path: 'organisation-editor/:id',
    component: OrganisationEditorComponent,
    data: { title: $localize`:@@globals.organisationEditor:Organisation Editor` },
    canActivate: [AuthGuard]
  },
  {
    path: 'matchmaking',
    component: MatchmakingComponent
  },
  {
    path: 'match-editor/:id',
    component: MatchEditorComponent,
    data: { title: $localize`:@@globals.matchEditor:Match Editor` },
    canActivate: [AuthGuard]
  },
  {
    path: 'login',
    component: LoginComponent,
    data: { title: $localize`:@@globals.login:Login` }
  },
  {
    path: 'error',
    component: ErrorComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
