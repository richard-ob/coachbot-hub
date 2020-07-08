import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { PagedMatchRequestDto, MatchFilters } from '../shared/model/dtos/paged-match-request-dto.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { MatchTypes } from '../shared/model/match-types.enum';
@Component({
    selector: 'app-upcoming-matches',
    templateUrl: './upcoming-matches.component.html',
    styleUrls: ['./upcoming-matches.component.scss']
})
export class UpcomingMatchesComponent implements OnInit {

    filters = new MatchFilters();
    matches: Match[];
    matchTypes = MatchTypes;
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private matchService: MatchService, private userPreferenceService: UserPreferenceService) { }

    ngOnInit() {
        this.filters.regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.filters.includeUpcoming = true;
        this.filters.includePast = false;
        this.loadPage(1);
    }

    loadPage(page: number) {
        this.matchService.getMatches(page, 10, this.filters).subscribe(response => {
            this.matches = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
        });
    }

}
