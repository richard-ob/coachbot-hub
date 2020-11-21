import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TournamentManagerComponent } from './tournament-manager.component';
import { TournamentMatchDaySlotManagerComponent } from './tournament-match-day-slot-manager/tournament-match-day-slot-manager.component';
import { TournamentActionsComponent } from './tournament-actions/tournament-actions.component';
import { TournamentStaffManagerComponent } from './tournament-staff-manager/tournament-staff-manager.component';
import { TournamentGroupsManagerComponent } from './tournament-groups-manager/tournament-groups-manager.component';
import { TournamentDetailsEditorComponent } from './tournament-details-editor/tournament-details-editor.component';

const routes: Routes = [
    {
        path: 'tournament-manager/:id',
        component: TournamentManagerComponent,
        children: [
            { path: '', redirectTo: 'details', pathMatch: 'full' },
            { path: 'details', component: TournamentDetailsEditorComponent },
            { path: 'groups', component: TournamentGroupsManagerComponent },
            { path: 'staff', component: TournamentStaffManagerComponent },
            { path: 'match-days', component: TournamentMatchDaySlotManagerComponent },
            { path: 'actions', component: TournamentActionsComponent },
        ],
        data: { title: $localize`:@@globals.tournamentManager:Tournament Manager` }
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class TournamentManagerRoutingModule { }
