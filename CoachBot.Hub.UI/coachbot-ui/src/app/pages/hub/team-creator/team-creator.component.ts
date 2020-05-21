import { Component } from '@angular/core';
import { TeamService } from '../shared/services/team.service';
import { Team } from '../shared/model/team.model';

@Component({
    selector: 'app-team-creator',
    templateUrl: './team-creator.component.html'
})
export class TeamCreatorComponent {

    teams: Team;

    constructor(private teamService: TeamService) {
    }
}
