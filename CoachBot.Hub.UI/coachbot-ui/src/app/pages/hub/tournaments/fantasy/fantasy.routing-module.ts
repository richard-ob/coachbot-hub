import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FantasyTeamEditorComponent } from './fantasy-team-editor/fantasy-team-editor.component';
import { FantasyTeamManagerComponent } from './fantasy-team-manager/fantasy-team-manager.component';
import { FantasyOverviewComponent } from './fantasy-overview/fantasy-overview.component';
import { FantasyTeamOverviewComponent } from './fantasy-team-overview/fantasy-team-overview.component';

const routes: Routes = [
    {
        path: 'fantasy',
        component: FantasyTeamManagerComponent,
        data: { title: 'Fantasy Manager' }
    },
    {
        path: 'fantasy/:tournamentId',
        component: FantasyOverviewComponent,
        data: { title: 'Fantasy Tournament Overview' }
    },
    {
        path: 'fantasy-editor/:id',
        component: FantasyTeamEditorComponent,
        data: { title: 'Fantasy Team Editor' }
    },
    {
        path: 'fantasy-overview/:id',
        component: FantasyTeamOverviewComponent,
        data: { title: 'Fantasy Team Overview' }
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class FantasyTeamManagerRoutingModule { }
