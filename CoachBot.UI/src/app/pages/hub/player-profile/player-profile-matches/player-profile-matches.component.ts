import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PlayerService } from '../../shared/services/player.service';
import { PlayerStatisticFilters } from '../../shared/model/dtos/paged-player-statistics-request-dto.model';
import { PlayerPositionMatchStatistics } from '../../shared/model/player-position-match-statistics.model';
import SortingUtils from '@shared/utilities/sorting-utilities';
import { RegionService } from '@pages/hub/shared/services/region.service';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';

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
    sortBy = 'Match.KickOff';
    sortOrder = 'ASC';
    timePeriod = 0;
    hasFiltered = false;
    isLoading = true;

    constructor(
        private playerService: PlayerService,
        private regionService: RegionService,
        private userPreferencesService: UserPreferenceService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.playerId = +params.get('id');
            this.filters.playerId = this.playerId;
            this.filters.regionId = this.userPreferencesService.getUserPreference(UserPreferenceType.Region);
            this.regionService.getRegions().subscribe(regions => {
                const region = regions.find(r => r.regionId === this.filters.regionId);
                this.filters.matchFormat = region.matchFormat;
                this.loadPage(1, this.sortBy);
            });
        });
    }

    loadPage(page: number, sortBy: string = null) {
        this.isLoading = true;
        if (page === this.currentPage) {
            this.sortOrder = SortingUtils.getSortOrder(this.sortBy, sortBy, this.sortOrder);
            this.sortBy = sortBy;
        }
        this.playerService.getPlayerPositionMatchStatistics(page, undefined, this.sortBy, this.sortOrder, this.filters)
            .subscribe(response => {
                this.matches = response.items;
                this.currentPage = response.page;
                this.totalPages = response.totalPages;
                this.totalItems = response.totalItems;
                this.isLoading = false;
            });
    }

    setFilters() {
        this.hasFiltered = true;
        this.loadPage(this.currentPage);
    }

    navigateToMatch(matchId: number) {
        this.router.navigate(['/match-overview', matchId]);
    }

}
