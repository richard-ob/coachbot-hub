import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { PagedMatchRequestDto } from '../shared/model/dtos/paged-match-request-dto.model';
@Component({
    selector: 'app-upcoming-matches',
    templateUrl: './upcoming-matches.component.html',
    styleUrls: ['./upcoming-matches.component.scss']
})
export class UpcomingMatchesComponent implements OnInit {

    filters = new PagedMatchRequestDto();
    matches: Match[];
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private matchService: MatchService) { }

    ngOnInit() {
        this.filters.regionId = 2;
        this.filters.includeUpcoming = true;
        this.filters.includePast = false;
        this.loadPage(1);
    }

    loadPage(page: number) {
        this.filters.page = page;
        this.matchService.getMatches(this.filters).subscribe(response => {
            this.matches = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
        });
    }

}
