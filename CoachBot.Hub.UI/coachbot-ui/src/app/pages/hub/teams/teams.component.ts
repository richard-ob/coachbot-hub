import { Component } from '@angular/core';
import { TeamService } from '../shared/services/team.service';
import { Team } from '../shared/model/team.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { TeamType } from '../shared/model/team-type.enum';

@Component({
    selector: 'app-teams',
    templateUrl: './teams.component.html',
    styleUrls: ['./teams.component.scss']
})
export class TeamsComponent {

    regionId: number;
    teams: Team[];
    teamTypes = TeamType;
    teamType: TeamType = TeamType.Club;
    isLoading = true;

    constructor(private teamService: TeamService, private userPreferenceService: UserPreferenceService) {
        this.regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.getTeams(this.teamType);
    }

    getTeams(teamType: TeamType) {
        this.isLoading = true;
        this.teamType = teamType;
        this.teamService.getTeams(this.regionId, this.teamType).subscribe(teams => {
            this.teams = teams;
            this.isLoading = false;
        });
    }
}
