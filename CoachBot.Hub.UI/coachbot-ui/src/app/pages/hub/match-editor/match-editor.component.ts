import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { ActivatedRoute } from '@angular/router';
import { Server } from 'selenium-webdriver/safari';
import { ServerService } from '../shared/services/server.service';

@Component({
    selector: 'app-match-editor',
    templateUrl: './match-editor.component.html'
})
export class MatchEditorComponent implements OnInit {

    match: Match;
    matchId: number;
    servers: Server[];
    showDatepicker = false;
    isLoading = true;
    /* TODO: Add custom match events to list, substitutions, half time, etc. See PL.com */
    constructor(private matchService: MatchService, private serverService: ServerService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.matchId = +params.get('id');
            this.serverService.getServers().subscribe(servers => {
                this.servers = servers;
                this.loadMatch();
            });
        });
    }

    loadMatch() {
        this.isLoading = true;
        this.matchService.getMatch(this.matchId).subscribe(match => {
            this.match = match;
            this.match.scheduledKickOff = new Date(this.match.scheduledKickOff);
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
