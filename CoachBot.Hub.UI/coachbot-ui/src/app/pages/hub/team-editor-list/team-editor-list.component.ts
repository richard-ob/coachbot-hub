import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { Player } from '../shared/model/player.model';
import { TeamRole } from '../shared/model/team-role.enum';
import { PlayerTeam } from '../shared/model/player-team.model';
import { PlayerTeamService } from '../shared/services/player-team.service';

@Component({
    selector: 'app-team-editor-list',
    templateUrl: './team-editor-list.component.html'
})
export class TeamEditorListComponent implements OnInit {

    player: Player;
    teamRoles = TeamRole;
    isLoading = true;

    constructor(private playerService: PlayerService, private playerTeamService: PlayerTeamService) { }

    ngOnInit() {
        this.playerService.getCurrentPlayer().subscribe(player => {
            this.player = player;
            this.isLoading = false;
        });
    }

    acceptInvite(playerTeam: PlayerTeam) {
        this.isLoading = true;
        playerTeam.isPending = false;
        this.playerTeamService.updatePlayerTeam(playerTeam).subscribe(() => this.isLoading = false);
    }

    leaveTeam(playerTeam: PlayerTeam) {
        this.isLoading = true;
        playerTeam.leaveDate = new Date();
        this.playerTeamService.updatePlayerTeam(playerTeam).subscribe(() => this.isLoading = false);
    }
}
