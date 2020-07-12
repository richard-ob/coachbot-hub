import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { CoreModule } from '@core/core.module';
import { TournamentManagerComponent } from './tournament-manager.component';
import { TournamentManagerRoutingModule } from './tournament-manager.routing-module';
import { TournamentMatchDaySlotManagerComponent } from './tournament-match-day-slot-manager/tournament-match-day-slot-manager.component';
import { TournamentActionsComponent } from './tournament-actions/tournament-actions.component';
import { TournamentStaffManagerComponent } from './tournament-staff-manager/tournament-staff-manager.component';
import { TournamentGroupsManagerComponent } from './tournament-groups-manager/tournament-groups-manager.component';
import {
    TournamentGroupTeamManagerComponent
} from './tournament-groups-manager/tournament-group-team-manager/tournament-group-team-manager.component';
import { TournamentDetailsEditorComponent } from './tournament-details-editor/tournament-details-editor.component';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { HubPipesModule } from '@pages/hub/shared/pipes/hub-pipes.module';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { NgPipesModule } from 'ngx-pipes';

@NgModule({
    declarations: [
        TournamentManagerComponent,
        TournamentDetailsEditorComponent,
        TournamentMatchDaySlotManagerComponent,
        TournamentGroupTeamManagerComponent,
        TournamentActionsComponent,
        TournamentStaffManagerComponent,
        TournamentGroupsManagerComponent
    ],
    imports: [
        CommonModule,
        CoreModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        TournamentManagerRoutingModule,
        SweetAlert2Module,
        HubPipesModule,
        TimepickerModule.forRoot(),
        NgPipesModule
    ]
})
export class TournamentManagerModule { }
