import { Component, OnInit, ViewChild } from '@angular/core';
import { MatchStatisticsService } from '@pages/hub/shared/services/match-statistics.service';
import { MatchStatistics } from '@pages/hub/shared/model/match-statistics.model';
import { MatchData } from '@pages/hub/match-overview/model/match-data.interface';
import { SwalPortalTargets, SwalComponent } from '@sweetalert2/ngx-sweetalert2';

@Component({
    selector: 'app-unlinked-match-statistics',
    templateUrl: './unlinked-match-statistics.component.html'
})
export class UnlinkedMatchStatisticsComponent implements OnInit {

    @ViewChild('matchDataModal') matchDataModal: SwalComponent;
    matchStatistics: MatchStatistics[];
    currentMatchData: MatchData;
    isLoading = true;

    constructor(private matchStatisticsService: MatchStatisticsService, public readonly swalTargets: SwalPortalTargets) { }

    ngOnInit() {
        this.getUnlinkedMatchData();
    }

    getUnlinkedMatchData() {
        this.matchStatisticsService.getUnlinkedMatchStatistics().subscribe(matchStatistics => {
            this.matchStatistics = matchStatistics;
            this.isLoading = false;
        });
    }

    openMatchDataModal(matchStatisticsId: number) {
        this.currentMatchData = this.matchStatistics.find(m => m.id === matchStatisticsId).matchData;
        this.matchDataModal.fire();
    }

    createMatch(matchStatisticsId: number) {
        this.isLoading = true;
        this.matchStatisticsService.createMatchFromMatchStatistics(matchStatisticsId).subscribe(() => {
            this.getUnlinkedMatchData();
        });
    }

}
