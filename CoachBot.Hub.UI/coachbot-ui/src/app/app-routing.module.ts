import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HeroComponent } from './pages/hero/hero.component';
import { MatchOverviewComponent } from './pages/hub/match-overview/match-overview.component';
import { ServerManagerComponent } from './pages/hub/server-manager/server-manager.component';
import { LoginComponent } from './pages/login/login.component';
import { ErrorComponent } from './pages/error/error.component';

const routes: Routes = [
  {
    path: '',
    component: HeroComponent
  },
  {
    path: 'match-overview',
    component: MatchOverviewComponent
  },
  {
    path: 'server-manager',
    component: ServerManagerComponent
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
