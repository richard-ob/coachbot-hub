import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TeamEditorComponent } from './team-editor.component';
import { TeamCreatorComponent } from './team-creator/team-creator.component';
import { TeamEditorDiscordIntegrationComponent } from './team-editor-discord-integration/team-editor-discord-integration.component';
import { TeamEditorSquadComponent } from './team-editor-player-list/team-editor-squad.component';
import { TeamEditorInfoComponent } from './team-editor-info/team-editor-info.component';

const routes: Routes = [
    {
        path: 'team/:id/manage',
        component: TeamEditorComponent,
        children: [
            { path: '', redirectTo: 'info', pathMatch: 'full' },
            { path: 'info', component: TeamEditorInfoComponent },
            { path: 'squad', component: TeamEditorSquadComponent },
            { path: 'discord', component: TeamEditorDiscordIntegrationComponent }
        ],
        data: { title: 'Team Editor' }
    },
    {
        path: 'create-team',
        component: TeamCreatorComponent,
        data: { title: 'Team Creator' }
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class TeamEditorRoutingModule { }
