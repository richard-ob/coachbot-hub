import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FantasyTeamManagerComponent } from './fantasy-team-manager.component';
import { FantasyTeamEditorComponent } from './fantasy-team-editor/fantasy-team-editor.component';

const routes: Routes = [
    {
        path: 'fantasy',
        component: FantasyTeamManagerComponent
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
