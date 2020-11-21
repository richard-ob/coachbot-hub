import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FantasyTeamEditorComponent } from './fantasy-team-editor/fantasy-team-editor.component';
import { FantasyTeamManagerComponent } from './fantasy-team-manager/fantasy-team-manager.component';
import { FantasyOverviewComponent } from './fantasy-overview/fantasy-overview.component';
import { FantasyTeamOverviewComponent } from './fantasy-team-overview/fantasy-team-overview.component';
import { AuthGuard } from '@core/guards/auth.guard';

const routes: Routes = [
    {
        path: 'fantasy',
        component: FantasyTeamManagerComponent,
        data: { title: $localize`:@@globals.fantasyManager:Fantasy Manager` },
        canActivate: [AuthGuard]
    },
    {
        path: 'fantasy/:tournamentId',
        component: FantasyOverviewComponent,
        data: { title: $localize`:@@globals.fantasyTournamentOverview:Fantasy Tournament Overview` }
    },
    {
        path: 'fantasy-editor/:id',
        component: FantasyTeamEditorComponent,
        data: { title: $localize`:@@globals.fantasyTeamEditor:Fantasy Team Editor` },
        canActivate: [AuthGuard]
    },
    {
        path: 'fantasy-overview/:id',
        component: FantasyTeamOverviewComponent,
        data: { title: $localize`:@@globals.fantasyTeamOverview:Fantasy Team Overview` }
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class FantasyTeamManagerRoutingModule { }
