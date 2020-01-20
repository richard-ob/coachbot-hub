import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HeroComponent } from './pages/hero/hero.component';
import { MatchOverviewComponent } from './pages/hub/match-overview/match-overview.component';

const routes: Routes = [
  {
    path: '',
    component: HeroComponent
  },
  {
    path: 'match-overview',
    component: MatchOverviewComponent
  }
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
