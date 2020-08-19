import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { ActivatedRoute } from '@angular/router';
import { Server } from 'selenium-webdriver/safari';
import { ServerService } from '../shared/services/server.service';
import { MatchStatistics } from '../shared/model/match-statistics.model';
import { TeamService } from '../shared/services/team.service';
import { Team } from '../shared/model/team.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';

@Component({
    selector: 'app-match-editor',
    templateUrl: './match-editor.component.html'
})
export class MatchEditorComponent implements OnInit {

    match: Match;
    matchId: number;
    servers: Server[];
    teams: Team[];
    matchStatistics: MatchStatistics;
    showDatepicker = false;
    isLoading = true;

    constructor(
        private matchService: MatchService,
        private serverService: ServerService,
        private teamService: TeamService,
        private userPreferenceService: UserPreferenceService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.matchId = +params.get('id');
            this.serverService.getServers().subscribe(servers => {
                this.servers = servers;
                this.teamService.getTeams(this.userPreferenceService.getUserPreference(UserPreferenceType.Region)).subscribe(teams => {
                    this.teams = teams;
                    this.loadMatch();
                });
            });
        });
    }

    loadMatch() {
        this.isLoading = true;
        this.matchService.getMatch(this.matchId).subscribe(match => {
            this.match = match;
            this.match.kickOff = new Date(this.match.kickOff);
            this.isLoading = false;
        });
    }

    updateMatch() {
        this.isLoading = true;
        this.matchService.updateMatch(this.match).subscribe(() => {
            this.loadMatch();
        });
    }

}
