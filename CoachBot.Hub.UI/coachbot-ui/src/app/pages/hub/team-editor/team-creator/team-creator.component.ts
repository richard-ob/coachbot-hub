import { Component } from '@angular/core';
import { Team } from '@pages/hub/shared/model/team.model';
import { TeamService } from '@pages/hub/shared/services/team.service';

@Component({
    selector: 'app-team-creator',
    templateUrl: './team-creator.component.html'
})
export class TeamCreatorComponent {

    team: Team = new Team();

    constructor(private teamService: TeamService) {
    }
}
