import { Component, OnInit, Input } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { MatchTypes } from '../shared/model/match-types.enum';
import { MatchFilters } from '../shared/model/dtos/paged-match-request-dto.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { Router } from '@angular/router';
@Component({
    selector: 'app-recent-matches',
    templateUrl: './recent-matches.component.html',
    styleUrls: ['./recent-matches.component.scss']
})
export class RecentMatchesComponent implements OnInit {

    @Input() playerId: number;
    @Input() teamId: number;
    @Input() tournamentId: number;
    @Input() includePast = true;
    @Input() includeUpcoming = false;
    @Input() showFilters = true;
    @Input() verticalPadding = true;
    filters = new MatchFilters();
    matchTypes = MatchTypes;
    matches: Match[];
    sortOrder = 'DESC';
    sortBy = 'KickOff';
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    hasFiltersApplied = false;
    isLoadingPage = false;

    constructor(private matchService: MatchService, private userPreferenceService: UserPreferenceService, private router: Router) { }

    ngOnInit() {
        this.filters.regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.filters.playerId = this.playerId;
        this.filters.teamId = this.teamId;
        this.filters.tournamentId = this.tournamentId;
        this.filters.includePast = this.includePast;
        this.filters.includeUpcoming = this.includeUpcoming;
        this.loadPage(1);
    }

    loadPage(page: number) {
        this.isLoadingPage = true;
        this.matchService.getMatches(page, 10, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
            this.matches = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
            this.isLoadingPage = false;
        });
    }

    navigateToMatchOverview(matchId: number) {
        this.router.navigate(['/match-overview/' + matchId]);
    }

    setFilters() {
        this.loadPage(this.currentPage);
        this.hasFiltersApplied = true;
    }
}
