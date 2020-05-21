import { Component } from '@angular/core';
import { TeamService } from '../shared/services/team.service';
import { Team } from '../shared/model/team.model';

@Component({
    selector: 'app-teams',
    templateUrl: './teams.component.html',
    styleUrls: ['./teams.component.scss']
})
export class TeamsComponent {

    teams: Team[];

    constructor(private teamService: TeamService) {
        this.teamService.getTeams(2).subscribe(teams => {
            this.teams = teams;
        });
    }
}
