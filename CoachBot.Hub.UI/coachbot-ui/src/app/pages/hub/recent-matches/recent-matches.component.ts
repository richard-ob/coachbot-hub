import { Component, OnInit, Input } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { MatchTypes } from '../shared/model/match-types.enum';
import { PagedMatchRequestDto } from '../shared/model/dtos/paged-match-request-dto.model';
@Component({
    selector: 'app-recent-matches',
    templateUrl: './recent-matches.component.html',
    styleUrls: ['./recent-matches.component.scss']
})
export class RecentMatchesComponent implements OnInit {

    @Input() playerId: number;
    @Input() teamId: number;
    @Input() tournamentEditionId: number;
    @Input() includePast = true;
    @Input() includeUpcoming = false;
    @Input() showFilters = true;
    filters = new PagedMatchRequestDto();
    matchTypes = MatchTypes;
    matches: Match[];
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private matchService: MatchService) { }

    ngOnInit() {
        this.filters.regionId = 2;
        this.filters.playerId = this.playerId;
        this.filters.teamId = this.teamId;
        this.filters.tournamentEditionId = this.tournamentEditionId;
        this.filters.includePast = this.includePast;
        this.filters.includeUpcoming = this.includeUpcoming;
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
