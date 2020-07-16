import { Component, Input } from '@angular/core';
import { Team } from '../../model/team.model';

@Component({
    selector: 'app-team-name',
    templateUrl: './team-name-display.component.html'
})
export class TeamNameDisplayComponent {

    @Input() team: Team;
    @Input() isHomeTeam = true;
    @Input() useTeamCodes = false;

}
