import { Component, OnInit, Input } from '@angular/core';
import { MatchService } from '@pages/hub/shared/services/match.service';
import { MatchStatisticsService } from '@pages/hub/shared/services/match-statistics.service';
import { MatchStatistics } from '@pages/hub/shared/model/match-statistics.model';
import { Match } from '@pages/hub/shared/model/match.model';

@Component({
    selector: 'app-match-data-uploader',
    templateUrl: './match-data-uploader.component.html'
})
export class MatchDataUploaderComponent implements OnInit {

    @Input() matchId: number;
    match: Match;
    matchStatistics: MatchStatistics;
    isLoading = true;
    isSubmittingStatistics = false;
    isProccessingStatistics = false;

    constructor(private matchService: MatchService, private matchStatisticsService: MatchStatisticsService) { }

    ngOnInit() {
        this.loadMatch();
    }

    loadMatch() {
        this.isLoading = true;
        this.matchService.getMatch(this.matchId).subscribe(match => {
            this.match = match;
            this.match.kickOff = new Date(this.match.kickOff);
            this.isLoading = false;
        });
    }

    updateMatchStatistics() {
        this.isSubmittingStatistics = true;
        this.matchStatisticsService.submitMatchStatistics(this.matchId, this.matchStatistics).subscribe(() => {
            this.isSubmittingStatistics = false;
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
