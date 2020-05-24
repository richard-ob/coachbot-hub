import { NgModule } from '@angular/core';
import { TeamRolePipe } from './team-role.pipe';
import { TournamentStaffRolePipe } from './tournament-staff-role.pipe';
import { TournamentTypePipe } from './tournament-type.pipe';
import { PositionGroupPipe } from './position-group.pipe';
import { ArrayFilterPipe } from './array-filter.pipe';
import { MatchOutcomePipe } from './match-outcome.pipe';

@NgModule({
    declarations: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe,
        ArrayFilterPipe,
        MatchOutcomePipe
    ],
    exports: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe,
        ArrayFilterPipe,
        MatchOutcomePipe
    ]
})
export class HubPipesModule { }
