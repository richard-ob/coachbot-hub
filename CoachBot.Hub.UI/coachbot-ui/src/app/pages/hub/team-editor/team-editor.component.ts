import { Component, OnInit } from '@angular/core';
import { PlayerTeamService } from '../shared/services/player-team.service';
import { PlayerService } from '../shared/services/player.service';
import { PlayerTeam } from '../shared/model/player-team.model';
import { Team } from '../shared/model/team.model';
import { RegionService } from '../shared/services/region.service';
import { Region } from '../shared/model/region.model';
import { TeamService } from '../shared/services/team.service';
import { DiscordService } from '../shared/services/discord.service';
import { ActivatedRoute } from '@angular/router';
import { TeamRole } from '../shared/model/team-role.enum';

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
        private regionService: RegionService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            const teamId = +params.get('id');
            this.playerService.getCurrentPlayer().subscribe(player => {
                if (player.teams.some(t => t.teamId === teamId && [TeamRole.Captain, TeamRole.ViceCaptain].some(tr => tr === t.teamRole))) {
                    this.teamService.getTeam(teamId).subscribe(team => {
                        this.team = team;
                        this.regionService.getRegions().subscribe(regions => {
                            this.regions = regions;
                            this.isLoading = false;
                        });
                    });
                } else {
                    this.accessDenied = true;
                    this.isLoading = false;
                }
            });
        });
    }

    saveTeamProfile() {
        this.isSaving = true;
        this.teamService.updateTeam(this.team).subscribe(() => {
            this.isSaving = false;
            this.teamService.getTeam(this.team.id).subscribe(team => {
                this.team = team;
            });
        });
    }

    updateBadgeImageId(assetImageId: number) {
        this.team.badgeImageId = assetImageId;
    }
}
