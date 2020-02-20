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

const routes: Routes = [
  {
    path: '',
    component: HeroComponent
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
    path: 'player-list',
    component: PlayerListComponent
  },
  {
    path: 'team-list',
    component: TeamListComponent
  },
  {
    path: 'player-profile/:id',
    component: PlayerProfileComponent
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
