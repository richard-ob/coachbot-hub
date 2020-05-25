import { NgModule } from '@angular/core';
import { TeamRolePipe } from './team-role.pipe';
import { TournamentStaffRolePipe } from './tournament-staff-role.pipe';
import { TournamentTypePipe } from './tournament-type.pipe';
import { PositionGroupPipe } from './position-group.pipe';
import { ArrayFilterPipe } from './array-filter.pipe';
import { MatchOutcomePipe } from './match-outcome.pipe';
import { TeamTypePipe } from './team-type.pipe';

@NgModule({
    declarations: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe,
        ArrayFilterPipe,
        MatchOutcomePipe,
        TeamTypePipe
    ],
    exports: [
        TeamRolePipe,
        TournamentStaffRolePipe,
        TournamentTypePipe,
        PositionGroupPipe,
        ArrayFilterPipe,
        MatchOutcomePipe,
        TeamTypePipe
    ]
})
export class HubPipesModule { }
