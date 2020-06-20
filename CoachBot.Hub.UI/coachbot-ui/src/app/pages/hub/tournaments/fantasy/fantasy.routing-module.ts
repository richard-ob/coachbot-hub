import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FantasyTeamEditorComponent } from './fantasy-team-editor/fantasy-team-editor.component';
import { FantasyTeamManagerComponent } from './fantasy-team-manager/fantasy-team-manager.component';
import { FantasyOverviewComponent } from './fantasy-overview/fantasy-overview.component';

const routes: Routes = [
    {
        path: 'fantasy',
        component: FantasyTeamManagerComponent
    },
    {
        path: 'fantasy/:tournamentId',
        component: FantasyOverviewComponent
    },
    {
        path: 'fantasy-editor/:id',
        component: FantasyTeamEditorComponent
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class FantasyTeamManagerRoutingModule { }
