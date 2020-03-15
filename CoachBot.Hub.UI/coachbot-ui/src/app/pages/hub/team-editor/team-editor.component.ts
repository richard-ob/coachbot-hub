import { Component, OnInit } from '@angular/core';
import { TeamService } from '../shared/services/team.service';
import { ActivatedRoute } from '@angular/router';
import { PlayerTeamService } from '../shared/services/player-team.service';
import { PlayerService } from '../shared/services/player.service';
import { PlayerTeam } from '../shared/model/player-team.model';
import { Team } from '../shared/model/team.model';
import { RegionService } from '../shared/services/region.service';
import { Region } from '../shared/model/region.model';

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

    constructor(
        private playerTeamservice: PlayerTeamService,
        private teamService: TeamService,
        private playerService: PlayerService,
        private regionService: RegionService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.playerService.getCurrentPlayer().subscribe(player => {
            this.playerTeamservice.getForPlayer(player.id).subscribe((teams) => {
                this.teams = teams;
                this.team = this.teams[0].team;
                this.playerTeamservice.getForTeam(this.team.id).subscribe(teamPlayers => {
                    this.teamPlayers = teamPlayers;
                    this.regionService.getRegions().subscribe(regions => {
                        this.regions = regions;
                        this.isLoading = false;
                    });
                });
            });
        });
    }
}
