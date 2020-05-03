import { Component, OnInit } from '@angular/core';
import { PlayerTeamService } from '../shared/services/player-team.service';
import { PlayerService } from '../shared/services/player.service';
import { PlayerTeam } from '../shared/model/player-team.model';
import { Team } from '../shared/model/team.model';
import { RegionService } from '../shared/services/region.service';
import { Region } from '../shared/model/region.model';
import { TeamService } from '../shared/services/team.service';
import { DiscordService } from '../shared/services/discord.service';

@Component({
    selector: 'app-team-editor',
    templateUrl: './team-editor.component.html',
    styleUrls: ['./team-editor.component.scss']
})
export class TeamEditorComponent implements OnInit {

    teams: PlayerTeam[];
    team: Team;
    teamPlayers: PlayerTeam[];
    regions: Region[];
    isLoading = true;
    isSaving = false;
    noTeamsAvailable = false;

    constructor(
        private playerTeamservice: PlayerTeamService,
        private playerService: PlayerService,
        private teamService: TeamService,
        private regionService: RegionService,
        private discordService: DiscordService
    ) { }

    ngOnInit() {
        this.playerService.getCurrentPlayer().subscribe(player => {
            this.playerTeamservice.getForPlayer(player.id).subscribe((teams) => {
                this.teams = teams;
                if (this.teams && this.teams.length) {
                    this.team = this.teams[0].team;
                } else {
                    this.noTeamsAvailable = true;
                }
                this.regionService.getRegions().subscribe(regions => {
                    this.regions = regions;
                    this.isLoading = false;
                });
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
