import { Component, OnInit, Input } from '@angular/core';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { PlayerStatisticFilters } from '@pages/hub/shared/model/dtos/paged-player-statistics-request-dto.model';
import SortingUtils from '@shared/utilities/sorting-utilities';
import { Router } from '@angular/router';
import { PlayerMatchStatistics } from '@pages/hub/shared/model/player-match-statistics.model';
import { PlayerPositionMatchStatistics } from '@pages/hub/shared/model/player-position-match-statistics.model';

@Component({
    selector: 'app-match-overview-player-statistics',
    templateUrl: './match-overview-player-statistics.component.html'
})
export class MatchOverviewPlayerStatisticsComponent implements OnInit {

    @Input() matchId: number;
    filters = new PlayerStatisticFilters();
    players: PlayerMatchStatistics[] | PlayerPositionMatchStatistics[];
    currentPage = 1;
    sortBy: string = null;
    sortOrder = 'ASC';
    showMoreStats = false;
    showAggregateStats = true;
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
        if (this.showAggregateStats) { // Aggregated for whole match (no positions)
            this.loadAggregateStats();
        } else { // By Position
            this.loadPositionStats();
        }
    }

    loadAggregateStats() {
        if (this.sortBy === 'Position.Name') {
            this.sortBy = null;
            this.sortOrder = null;
        }
        this.playerService.getPlayerMatchStatistics(1, 50, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
            this.players = response.items;
            this.currentPage = response.page;
            this.isLoading = false;
            this.isSorting = false;
        });
    }

    loadPositionStats() {
        this.playerService.getPlayerPositionMatchStatistics(1, 50, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
            this.players = response.items;
            this.currentPage = response.page;
            this.isLoading = false;
            this.isSorting = false;
        });
    }

    toggleStatAggregation() {
        this.isLoading = true;
        this.showAggregateStats = !this.showAggregateStats;
        if (this.showAggregateStats) {
            this.loadAggregateStats();
        } else {
            this.loadPositionStats();
        }
    }

    navigateToPlayerProfile(playerId: number) {
        this.router.navigate(['/player-profile/' + playerId]);
    }

}
