import { Component } from '@angular/core';
import { TeamService } from '../shared/services/team.service';
import { Team } from '../shared/model/team.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';

@Component({
    selector: 'app-teams',
    templateUrl: './teams.component.html',
    styleUrls: ['./teams.component.scss']
})
export class TeamsComponent {

    teams: Team[];
    isLoading = true;

    constructor(private teamService: TeamService, private userPreferenceService: UserPreferenceService) {
        const regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.teamService.getTeams(regionId).subscribe(teams => {
            this.teams = teams;
            this.isLoading = false;
        });
    }
}
