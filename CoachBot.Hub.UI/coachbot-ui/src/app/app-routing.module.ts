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
import { TeamEditorComponent } from './pages/hub/team-editor/team-editor.component';
import { TournamentCreatorComponent } from './pages/hub/tournaments/tournament-creator/tournament-creator.component';
import { TournamentEditorComponent } from './pages/hub/tournaments/tournament-editor/tournament-editor.component';
import { TournamentEditionManagerComponent } from './pages/hub/tournaments/tournament-edition-manager/tournament-edition-manager.component';
import { MatchEditorComponent } from './pages/hub/match-editor/match-editor.component';
import { UpcomingMatchesComponent } from './pages/hub/upcoming-matches/upcoming-matches.component';
import { CurrentTournamentsComponent } from './pages/hub/tournaments/current-tournaments/current-tournaments.component';
import { MatchmakingComponent } from './pages/matchmaking/matchmaking.component';
import { AboutComponent } from '@pages/about/about.component';
import { ServerManualComponent } from '@pages/server-manual/server-manual.component';
import { NewsComponent } from '@pages/news/news.component';
import { TeamsComponent } from '@pages/hub/teams/teams.component';
import { TeamCreatorComponent } from '@pages/hub/team-creator/team-creator.component';

const routes: Routes = [
  {
    path: '',
    component: HeroComponent
  },
  {
    path: 'manual',
    component: ManualComponent
  },
  {
    path: 'server-manual',
    component: ServerManualComponent
  },
  {
    path: 'news',
    component: NewsComponent
  },
  {
    path: 'team',
    component: TeamComponent
  },
  {
    path: 'about',
    component: AboutComponent
  },
  {
    path: 'match-overview/:id',
    component: MatchOverviewComponent
  },
  {
    path: 'server-manager',
    component: ServerManagerComponent
  },
  {
    path: 'region-manager',
    component: RegionManagerComponent
  },
  {
    path: 'recent-matches',
    component: RecentMatchesComponent
  },
  {
    path: 'upcoming-matches',
    component: UpcomingMatchesComponent
  },
  {
    path: 'player-list',
    component: PlayerListComponent
  },
  {
    path: 'team-list',
    component: TeamListComponent
  },
  {
    path: 'edit-profile',
    component: ProfileEditorComponent
  },
  {
    path: 'edit-team',
    component: TeamEditorComponent
  },
  {
    path: 'create-team',
    component: TeamCreatorComponent
  },
  {
    path: 'teams',
    component: TeamsComponent
  },
  {
    path: 'validate-steamid',
    component: SteamIDValidatorComponent
  },
  {
    path: 'tournament-creator',
    component: TournamentCreatorComponent
  },
  {
    path: 'tournament-editor/:id',
    component: TournamentEditorComponent
  },
  {
    path: 'tournament-edition-manager/:id',
    component: TournamentEditionManagerComponent
  },
  {
    path: 'tournaments',
    component: CurrentTournamentsComponent
  },
  {
    path: 'matchmaking',
    component: MatchmakingComponent
  },
  {
    path: 'match-editor/:id',
    component: MatchEditorComponent
  },
  {
    path: 'login',
    component: LoginComponent
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
