import { NgModule } from '@angular/core';
import { TeamRolePipe } from './team-role.pipe';
import { TournamentStaffRolePipe } from './tournament-staff-role.pipe';
import { TournamentTypePipe } from './tournament-type.pipe';
import { PositionGroupPipe } from './position-group.pipe';

@NgModule({
    declarations: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe
    ],
    exports: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe
    ]
})
export class HubPipesModule { }
