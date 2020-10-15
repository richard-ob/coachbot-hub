import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { ActivatedRoute } from '@angular/router';
import { Server } from 'selenium-webdriver/safari';
import { ServerService } from '../shared/services/server.service';
import { TeamService } from '../shared/services/team.service';
import { Team } from '../shared/model/team.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { MatchStatisticsService } from '../shared/services/match-statistics.service';
import { MapService } from '../shared/services/map.service';
import { Map } from '../shared/model/map.model';

@Component({
    selector: 'app-match-editor',
    templateUrl: './match-editor.component.html'
})
export class MatchEditorComponent implements OnInit {

    match: Match;
    matchId: number;
    servers: Server[];
    teams: Team[];
    maps: Map[];
    homeGoalsOverride: number;
    awayGoalsOverride: number;
    matchToken: string;
    showDatepicker = false;
    isLoading = true;

    constructor(
        private matchService: MatchService,
        private serverService: ServerService,
        private teamService: TeamService,
        private matchStatisticsService: MatchStatisticsService,
        private userPreferenceService: UserPreferenceService,
        private mapService: MapService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.matchId = +params.get('id');
            this.serverService.getServers().subscribe(servers => {
                this.servers = servers;
                this.teamService.getTeams(this.userPreferenceService.getUserPreference(UserPreferenceType.Region)).subscribe(teams => {
                    this.teams = teams;
                    this.mapService.getMaps().subscribe(maps => {
                        this.maps = maps;
                        this.loadMatch();
                    });
                });
            });
        });
    }

    loadMatch() {
        this.isLoading = true;
        this.matchService.getMatch(this.matchId).subscribe(match => {
            this.match = match;
            this.match.kickOff = new Date(this.match.kickOff);
            if (this.match.matchStatistics) {
                this.homeGoalsOverride = this.match.matchStatistics.homeGoals;
                this.awayGoalsOverride = this.match.matchStatistics.awayGoals;
            }
            this.matchToken = this.generateMatchToken();
            this.isLoading = false;
        });
    }

    updateMatch() {
        this.isLoading = true;
        this.matchService.updateMatch(this.match).subscribe(() => {
            this.loadMatch();
        });
    }

    submitMatchResultOverride() {
        this.isLoading = true;
        this.matchStatisticsService.createMatchResultOverride(
            this.matchId, this.homeGoalsOverride, this.awayGoalsOverride
        ).subscribe(() => {
            this.isLoading = false;
        });
    }

    generateMatchToken() {
        if (this.match.teamHome && this.match.teamAway && this.match.server) {
            const token = `${this.match.server.address}_${this.match.id}_${this.match.teamHome.teamCode}_${this.match.teamAway.teamCode}`;
            return btoa(token);
        }

        return;
    }

}
