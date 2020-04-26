import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
@Component({
    selector: 'app-upcoming-matches',
    templateUrl: './upcoming-matches.component.html',
    styleUrls: ['./upcoming-matches.component.scss']
})
export class UpcomingMatchesComponent implements OnInit {

    matches: Match[];
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private matchService: MatchService) { }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(page: number) {
        this.matchService.getMatches(2, page, undefined, undefined, true).subscribe(response => {
            this.matches = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
        });
    }

}
