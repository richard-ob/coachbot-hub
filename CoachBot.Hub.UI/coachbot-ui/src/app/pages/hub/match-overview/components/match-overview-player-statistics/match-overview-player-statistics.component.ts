import { Component, OnInit, Input } from '@angular/core';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { PlayerStatisticFilters } from '@pages/hub/shared/model/dtos/paged-player-statistics-request-dto.model';
import { PlayerPositionMatchStatistics } from '@pages/hub/shared/model/player-match-statistics.model';
import SortingUtils from '@shared/utilities/sorting-utilities';
import { Router } from '@angular/router';

@Component({
    selector: 'app-match-overview-player-statistics',
    templateUrl: './match-overview-player-statistics.component.html'
})
export class MatchOverviewPlayerStatisticsComponent implements OnInit {

    @Input() matchId: number;
    filters = new PlayerStatisticFilters();
    players: PlayerPositionMatchStatistics[];
    currentPage = 1;
    sortBy: string = null;
    sortOrder = 'ASC';
    showMoreStats = false;
    isLoading = true;
    isSorting = false;

    constructor(private playerService: PlayerService, private router: Router) { }

    ngOnInit() {
        this.filters.matchId = this.matchId;
        this.loadResults();
    }

    loadResults(sortBy: string = null) {
        this.isSorting = true;
        this.sortOrder = SortingUtils.getSortOrder(this.sortBy, sortBy, this.sortOrder);
        this.sortBy = sortBy;
        this.playerService.getPlayerMatchStatistics(1, 50, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
            this.players = response.items;
            this.currentPage = response.page;
            this.isLoading = false;
            this.isSorting = false;
        });
    }

    navigateToPlayerProfile(playerId: number) {
        this.router.navigate(['/player-profile/' + playerId]);
    }

}
