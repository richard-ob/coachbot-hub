import { Component, OnInit, Input } from '@angular/core';
import { PlayerTeamService } from '../../shared/services/player-team.service';
import { PlayerTeam } from '../../shared/model/player-team.model';

@Component({
    selector: 'app-team-editor-squad',
    templateUrl: './team-editor-squad.component.html',
    styleUrls: ['./team-editor-squad.component.scss']
})
export class TeamEditorSquadComponent implements OnInit {

    @Input() teamId: number;
    teamPlayers: PlayerTeam[];
    isLoading = true;
    isUpdating = true;
    isAddPlayerWizardOpen = false;

    constructor(private playerTeamService: PlayerTeamService) { }

    ngOnInit() {
        this.isLoading = true;
        this.loadPlayers();
    }

    loadPlayers() {
        this.playerTeamService.getForTeam(this.teamId).subscribe(teamPlayers => {
            this.teamPlayers = teamPlayers;
            this.isLoading = false;
            this.isUpdating = false;
        });
    }

    kickPlayer(playerTeam: PlayerTeam) {
        this.isUpdating = true;
        playerTeam.leaveDate = new Date();
        this.playerTeamService.updatePlayerTeam(playerTeam).subscribe(() => this.loadPlayers());
    }

    toggleAddPlayerWizard() {
        this.isAddPlayerWizardOpen = !this.isAddPlayerWizardOpen;
    }
}
