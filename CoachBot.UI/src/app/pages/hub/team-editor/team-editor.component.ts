import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { Team } from '../shared/model/team.model';
import { Region } from '../shared/model/region.model';
import { TeamService } from '../shared/services/team.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TeamRole } from '../shared/model/team-role.enum';
import { PlayerHubRole } from '../shared/model/player-hub-role.enum';

@Component({
    selector: 'app-team-editor',
    templateUrl: './team-editor.component.html',
    styleUrls: ['./team-editor.component.scss']
})
export class TeamEditorComponent implements OnInit {

    team: Team;
    regions: Region[];
    isLoading = true;
    isSaving = false;
    accessDenied = false;

    constructor(
        private playerService: PlayerService,
        private teamService: TeamService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            const teamId = +params.get('id');

            this.playerService.getCurrentPlayer().subscribe(player => {
                if (player.teams.some(t => t.teamId === teamId && [TeamRole.Captain, TeamRole.ViceCaptain].some(tr => tr === t.teamRole))
                    || player.hubRole === PlayerHubRole.Owner || player.hubRole === PlayerHubRole.Administrator) {
                    this.teamService.getTeam(teamId).subscribe(team => {
                        this.team = team;
                        this.isLoading = false;
                    });
                } else {
                    this.accessDenied = true;
                    this.isLoading = false;
                }
            });
        });
    }
}
