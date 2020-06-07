import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PlayerService } from '../../shared/services/player.service';
import { PlayerStatisticFilters } from '../../shared/model/dtos/paged-player-statistics-request-dto.model';
import { PlayerPositionMatchStatistics } from '../../shared/model/player-match-statistics.model';
import SortingUtils from '@shared/utilities/sorting-utilities';

@Component({
    selector: 'app-player-profile-matches',
    templateUrl: './player-profile-matches.component.html'
})
export class PlayerProfileMatchesComponent implements OnInit {

    playerId: number;
    matches: PlayerPositionMatchStatistics[];
    filters = new PlayerStatisticFilters();
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    sortBy: string = null;
    sortOrder = 'ASC';
    timePeriod = 0;
    isLoading = true;

    constructor(private playerService: PlayerService, private route: ActivatedRoute, private router: Router) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.playerId = +params.get('id');
            this.filters.playerId = this.playerId;
            this.loadPage(1);
        });
    }

    loadPage(page: number, sortBy: string = null) {
        this.isLoading = true;
        this.sortOrder = SortingUtils.getSortOrder(this.sortBy, sortBy, this.sortOrder);
        this.sortBy = sortBy;
        this.playerService.getPlayerMatchStatistics(page, undefined, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
            this.matches = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
            this.isLoading = false;
        });
    }

    navigateToMatch(matchId: number) {
        this.router.navigate(['/match-overview', matchId]);
    }

}
