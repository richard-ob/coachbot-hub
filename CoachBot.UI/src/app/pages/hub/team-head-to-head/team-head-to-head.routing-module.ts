import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TeamHeadToHeadSelectorComponent } from './team-head-to-head-selector/team-head-to-head-selector.component';
import { TeamHeadToHeadComponent } from './team-head-to-head.component';

const routes: Routes = [
    {
        path: 'team-head-to-head/:teamOneCode/:teamTwoCode',
        component: TeamHeadToHeadComponent,
        data: { title: $localize`:@@globals.teamHeadToHead:Team Head To Head` }
    },
    {
        path: 'team-head-to-head',
        component: TeamHeadToHeadSelectorComponent,
        data: { title: $localize`:@@globals.teamHeadToHead:Team Head To Head` }
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class TeamHeadToHeadRoutingModule { }
