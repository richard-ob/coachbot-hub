import { Component, OnInit, Input } from '@angular/core';
import { PlayerTeamService } from '../../shared/services/player-team.service';
import { PlayerTeam } from '../../shared/model/player-team.model';
import { TeamRole } from '@pages/hub/shared/model/team-role.enum';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '@pages/hub/shared/services/player.service';

@Component({
    selector: 'app-team-editor-squad',
    templateUrl: './team-editor-squad.component.html',
    styleUrls: ['./team-editor-squad.component.scss']
})
export class TeamEditorSquadComponent implements OnInit {

    @Input() teamId: number;
    currentPlayer: Player;
    teamPlayers: PlayerTeam[];
    teamRoles = TeamRole;
    isLoading = true;
    isUpdating = true;
    isAddPlayerWizardOpen = false;

    constructor(
        private playerTeamService: PlayerTeamService,
        private playerService: PlayerService,
        private snackBar: MatSnackBar,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.isLoading = true;
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.teamId = +params.get('id');
            this.playerService.getCurrentPlayer().subscribe(currentPlayer => {
                this.currentPlayer = currentPlayer;
                this.loadPlayers();
            });
        });
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

    updatePlayerRole(playerTeam: PlayerTeam, teamRole: TeamRole) {
        this.isUpdating = true;
        playerTeam.teamRole = teamRole;
        this.playerTeamService.updatePlayerTeam(playerTeam).subscribe(() => {
            this.loadPlayers();
            this.snackBar.open('Role changed for ' + playerTeam.player.name, 'Dismiss');
        });
    }

    toggleAddPlayerWizard() {
        this.isAddPlayerWizardOpen = !this.isAddPlayerWizardOpen;
    }
}
