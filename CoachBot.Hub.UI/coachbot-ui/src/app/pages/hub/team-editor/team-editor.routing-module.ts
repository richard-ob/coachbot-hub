import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TeamEditorComponent } from './team-editor.component';
import { TeamCreatorComponent } from './team-creator/team-creator.component';

const routes: Routes = [
    {
        path: 'team/:id/manage',
        component: TeamEditorComponent
    },
    {
        path: 'create-team',
        component: TeamCreatorComponent
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class TeamEditorRoutingModule { }
