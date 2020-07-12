import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { ActivatedRoute } from '@angular/router';
import { Server } from 'selenium-webdriver/safari';
import { ServerService } from '../shared/services/server.service';
import { MatchStatistics } from '../shared/model/match-statistics.model';

@Component({
    selector: 'app-match-editor',
    templateUrl: './match-editor.component.html'
})
export class MatchEditorComponent implements OnInit {

    match: Match;
    matchId: number;
    servers: Server[];
    matchStatistics: MatchStatistics;
    showDatepicker = false;
    isLoading = true;
    isSubmittingStatistics = false;
    isProccessingStatistics = false;

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

    updateMatchStatistics() {
        this.isLoading = true;
        this.matchService.submitMatchStatistics(this.matchId, this.matchStatistics).subscribe(() => {
            this.isLoading = false;
            this.isProccessingStatistics = true;
        });
    }

    fileSelected(event: any) {
        const file = event.target.files[0];
        const fileReader = new FileReader();
        fileReader.addEventListener('load', () => {
            try {
                this.matchStatistics = JSON.parse(fileReader.result as string);
            } catch (e) {
                return;
            }
        });
        fileReader.readAsText(file, '"UTF-8');
    }
}
