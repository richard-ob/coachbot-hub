import { NgModule } from '@angular/core';
import { TeamRolePipe } from './team-role.pipe';

@NgModule({
    declarations: [
        TeamRolePipe
    ],
    exports: [
        TeamRolePipe
    ]
})
export class HubPipesModule { }
