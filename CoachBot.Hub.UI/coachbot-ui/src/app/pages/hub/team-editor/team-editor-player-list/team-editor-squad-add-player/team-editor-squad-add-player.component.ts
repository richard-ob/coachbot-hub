import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Player } from '@pages/hub/shared/model/player.model';
import { PlayerTeamService } from '@pages/hub/shared/services/player-team.service';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { TeamRole } from '@pages/hub/shared/model/team-role.enum';
import { PlayerTeam } from '@pages/hub/shared/model/player-team.model';

@Component({
    selector: 'app-team-editor-squad-add-player',
    templateUrl: './team-editor-squad-add-player.component.html'
})
export class TeamEditorSquadAddPlayerComponent {

    @Output() wizardClosed = new EventEmitter<void>();
    @Input() teamId: number;
    playerSearchResults: Player[];
    player: Player;
    role: TeamRole;
    teamRoles = TeamRole;
    playerAddFailure = false;
    isSaving = false;
    isSearching = false;

    constructor(private playerTeamService: PlayerTeamService, private playerService: PlayerService) { }

    addPlayer() {
        this.isSaving = true;
        const playerTeam: PlayerTeam = {
            teamId: this.teamId,
            teamRole: this.role,
            playerId: this.player.id
        };
        this.playerTeamService.createPlayerTeam(playerTeam).subscribe(
            () => {
                this.isSaving = false;
                this.wizardClosed.emit();
            },
            error => {
                this.playerAddFailure = true;
                this.isSaving = false;
            }
        );
    }

    selectPlayer(player: Player) {
        this.playerAddFailure = false;
        this.player = player;
        this.playerSearchResults = null;
    }

    search(playerName: string) {
        this.player = null;
        this.playerAddFailure = false;
        this.isSearching = true;
        this.playerService.searchPlayerByName(playerName).subscribe(
            players => {
                this.playerSearchResults = players;
                this.isSearching = false;
            }
        );
    }
}
