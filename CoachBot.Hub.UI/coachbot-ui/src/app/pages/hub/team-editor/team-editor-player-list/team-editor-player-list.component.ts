import { Component, OnInit, Input } from '@angular/core';
import { PlayerTeamService } from '../../shared/services/player-team.service';
import { PlayerTeam } from '../../shared/model/player-team.model';

@Component({
    selector: 'app-team-editor-player-list',
    templateUrl: './team-editor-player-list.component.html',
    styleUrls: ['./team-editor-player-list.component.scss']
})
export class TeamEditorPlayerListComponent implements OnInit {

    @Input() teamId: number;
    teamPlayers: PlayerTeam[];
    isLoading = true;

    constructor(private playerTeamservice: PlayerTeamService) { }

    ngOnInit() {
        this.playerTeamservice.getForTeam(1).subscribe(teamPlayers => {
            this.teamPlayers = teamPlayers;
            this.isLoading = false;
        });
    }
}
