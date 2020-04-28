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
import { PlayerProfileComponent } from './pages/hub/player-profile/player-profile.component';
import { TeamListComponent } from './pages/hub/team-list/team-list.component';
import { ProfileEditorComponent } from './pages/hub/profile-editor/profile-editor.component';
import { SteamIDValidatorComponent } from './pages/hub/profile-editor/steam-id-validator.component';
import { TeamProfileComponent } from './pages/hub/team-profile/team-profile.component';
import { ManualComponent } from './pages/manual/manual.component';
import { TeamComponent } from './pages/team/team.component';
import { TeamEditorComponent } from './pages/hub/team-editor/team-editor.component';
import { TournamentCreatorComponent } from './pages/hub/tournaments/tournament-creator/tournament-creator.component';
import { TournamentEditorComponent } from './pages/hub/tournaments/tournament-editor/tournament-editor.component';
import { TournamentEditionManagerComponent } from './pages/hub/tournaments/tournament-edition-manager/tournament-edition-manager.component';
import { MatchEditorComponent } from './pages/hub/match-editor/match-editor.component';
import { UpcomingMatchesComponent } from './pages/hub/upcoming-matches/upcoming-matches.component';
import { PlayerProfileStatisticsComponent } from './pages/hub/player-profile/player-profile-statistics/player-profile-statistics.component';
import { PlayerProfileMatchesComponent } from './pages/hub/player-profile/player-profile-matches/player-profile-matches.component';
import { PlayerTeamHistoryComponent } from './pages/hub/player-profile/player-team-history/player-team-history.component';
import { PlayerProfileTournamentsComponent } from './pages/hub/player-profile/player-profile-tournaments/player-profile-tournaments.component';
import { CurrentTournamentsComponent } from './pages/hub/tournaments/current-tournaments/current-tournaments.component';
import { MatchmakingComponent } from './pages/matchmaking/matchmaking.component';

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
    path: 'team',
    component: TeamComponent
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
    path: 'team-profile/:id',
    component: TeamProfileComponent
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
