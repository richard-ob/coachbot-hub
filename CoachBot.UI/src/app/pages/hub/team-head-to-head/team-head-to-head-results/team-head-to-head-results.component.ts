import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { MatchFilters } from '@pages/hub/shared/model/dtos/paged-match-request-dto.model';
import { Match } from '@pages/hub/shared/model/match.model';
import { MatchTypes } from '@pages/hub/shared/model/match-types.enum';
import { MatchService } from '@pages/hub/shared/services/match.service';
import { MatchOutcomeType } from '@pages/hub/shared/model/match-outcome-type.enum';

@Component({
    selector: 'app-team-head-to-head-results',
    templateUrl: './team-head-to-head-results.component.html',
    styleUrls: ['./team-head-to-head-results.component.scss']
})
export class TeamHeadToHeadResultsComponent implements OnInit {

    @Input() teamId: number;
    @Input() oppositionTeamId: number;
    @Input() showFilters = true;
    @Input() verticalPadding = true;
    @Input() allRegions = false;
    @Input() sortOrder = 'DESC';
    filters = new MatchFilters();
    matchTypes = MatchTypes;
    matches: Match[];
    sortBy = 'KickOff';
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    hasFiltersApplied = false;
    isLoadingPage = false;

    constructor(private matchService: MatchService, private router: Router) { }

    ngOnInit() {
        this.filters.teamId = this.teamId;
        this.filters.includePast = true;
        this.filters.includeUpcoming = false;
        this.filters.oppositionTeamId = this.oppositionTeamId;
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

    getMatchOutcomeForTeam(match: Match) {
        if (!match.matchStatistics) {
            return;
        }

        const isHomeTeam = match.teamHomeId === this.teamId;
        const goalsScored = isHomeTeam ? match.matchStatistics.matchGoalsHome : match.matchStatistics.matchGoalsAway;
        const goalsConceded = isHomeTeam ? match.matchStatistics.matchGoalsAway : match.matchStatistics.matchGoalsHome;

        if (goalsScored > goalsConceded) {
            return MatchOutcomeType.Win;
        }
        if (goalsConceded > goalsScored) {
            return MatchOutcomeType.Loss;
        }

        return MatchOutcomeType.Draw;
    }
}
