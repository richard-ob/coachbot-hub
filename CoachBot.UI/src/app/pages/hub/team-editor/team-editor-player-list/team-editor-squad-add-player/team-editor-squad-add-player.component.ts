import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Player } from '@pages/hub/shared/model/player.model';
import { PlayerTeamService } from '@pages/hub/shared/services/player-team.service';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { TeamRole } from '@pages/hub/shared/model/team-role.enum';
import { PlayerTeam } from '@pages/hub/shared/model/player-team.model';
import { TeamType } from '@pages/hub/shared/model/team-type.enum';
import { TeamService } from '@pages/hub/shared/services/team.service';

@Component({
    selector: 'app-team-editor-squad-add-player',
    templateUrl: './team-editor-squad-add-player.component.html'
})
export class TeamEditorSquadAddPlayerComponent {

    @Output() wizardClosed = new EventEmitter<void>();
    @Output() playerAdded = new EventEmitter<void>();
    @Input() teamId: number;
    @Input() teamPlayers: Player[];
    teamType: TeamType;
    player: Player;
    role: TeamRole;
    teamRoles = TeamRole;
    playerAddFailure = false;
    playerAlreadyAdded = false;
    isSaving = false;

    constructor(private playerTeamService: PlayerTeamService, private playerService: PlayerService, private teamService: TeamService) { }

    addPlayer() {
        this.isSaving = true;
        this.playerAddFailure = false;
        this.playerAlreadyAdded = false;
        this.teamService.getTeam(this.teamId).subscribe(team => {
            this.playerService.getPlayerProfile(this.player.id).subscribe(playerProfile => {
                if (this.teamPlayers.some(tp => tp.id === this.player.id)) {
                    this.playerAlreadyAdded = true;
                    return;
                }
                if (!playerProfile.clubTeam ||
                    team.teamType !== TeamType.Club ||
                    (this.role === TeamRole.Loanee && playerProfile.clubTeamRole === TeamRole.Loaned)
                ) {
                    const playerTeam: PlayerTeam = {
                        teamId: this.teamId,
                        teamRole: this.role,
                        playerId: this.player.id
                    };
                    this.playerTeamService.createPlayerTeam(playerTeam).subscribe(() => {
                        this.isSaving = false;
                        this.wizardClosed.emit();
                        this.playerAdded.emit();
                    });
                } else {
                    this.playerAddFailure = true;
                    this.isSaving = false;
                }
            });
        });
    }

    selectPlayer(player: Player) {
        this.playerAddFailure = false;
        this.player = player;
    }
}
