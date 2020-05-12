import { NgModule } from '@angular/core';
import { TeamRolePipe } from './team-role.pipe';
import { TournamentStaffRolePipe } from './tournament-staff-role.pipe';
import { TournamentTypePipe } from './tournament-type.pipe';
import { PositionGroupPipe } from './position-group.pipe';
import { ArrayFilterPipe } from './array-filter.pipe';

@NgModule({
    declarations: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe,
        ArrayFilterPipe
    ],
    exports: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe,
        ArrayFilterPipe
    ]
})
export class HubPipesModule { }
