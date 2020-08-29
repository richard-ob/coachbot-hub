import { Component } from '@angular/core';
import { TeamService } from '../shared/services/team.service';
import { Team } from '../shared/model/team.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { TeamType } from '../shared/model/team-type.enum';
import { PlayerService } from '../shared/services/player.service';
import { Player } from '../shared/model/player.model';
import { PlayerHubRole } from '../shared/model/player-hub-role.enum';

@Component({
    selector: 'app-teams',
    templateUrl: './teams.component.html',
    styleUrls: ['./teams.component.scss']
})
export class TeamsComponent {

    currentPlayer: Player;
    regionId: number;
    teams: Team[];
    teamTypes = TeamType;
    hubRoles = PlayerHubRole;
    teamType: TeamType = TeamType.Club;
    isLoading = true;

    constructor(
        private teamService: TeamService,
        private playerService: PlayerService,
        private userPreferenceService: UserPreferenceService
    ) {
        this.regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.currentPlayer = this.playerService.currentPlayer;
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

    deleteTeam(teamId: number) {
        this.isLoading = true;
        this.teamService.deleteTeam(teamId).subscribe(() => {
            this.getTeams(this.teamType);
        });
    }
}
