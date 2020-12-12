import { Component, OnInit, Input } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
import { MatchTypes } from '../shared/model/match-types.enum';
import { MatchFilters } from '../shared/model/dtos/paged-match-request-dto.model';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { Router } from '@angular/router';
import { MatchOutcomeType } from '../shared/model/match-outcome-type.enum';
import { RegionService } from '../shared/services/region.service';
import { TournamentService } from '../shared/services/tournament.service';
import { Team } from '../shared/model/team.model';
@Component({
    selector: 'app-recent-matches',
    templateUrl: './recent-matches.component.html',
    styleUrls: ['./recent-matches.component.scss']
})
export class RecentMatchesComponent implements OnInit {

    @Input() playerId: number;
    @Input() teamId: number;
    @Input() oppositionTeamId: number;
    @Input() tournamentId: number;
    @Input() includePast = true;
    @Input() includeUpcoming = false;
    @Input() includePlaceholders = false;
    @Input() showFilters = true;
    @Input() verticalPadding = true;
    @Input() allRegions = false;
    @Input() sortOrder = 'DESC';
    filters = new MatchFilters();
    matchTypes = MatchTypes;
    matches: Match[];
    teams: Team[];
    sortBy = 'KickOff';
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    hasFiltersApplied = false;
    isLoadingPage = false;

    constructor(
        private matchService: MatchService,
        private userPreferenceService: UserPreferenceService,
        private regionService: RegionService,
        private tournamentService: TournamentService,
        private router: Router
    ) {
    }

    ngOnInit() {
        this.filters.playerId = this.playerId;
        this.filters.teamId = this.teamId;
        this.filters.tournamentId = this.tournamentId;
        this.filters.includePast = this.includePast;
        this.filters.includeUpcoming = this.includeUpcoming;
        this.filters.includePlaceholders = this.includePlaceholders;
        this.filters.oppositionTeamId = this.oppositionTeamId;
        if (!this.allRegions) {
            this.filters.regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
            this.regionService.getRegions().subscribe((regions) => {
                const region = regions.find(r => r.regionId === this.filters.regionId);
                this.filters.matchFormat = region.matchFormat;
                if (this.filters.tournamentId) {
                    this.tournamentService.getTournamentTeams(this.tournamentId).subscribe(tournamentTeams => {
                        this.teams = tournamentTeams;
                        this.loadPage(1);
                    });
                } else {
                    this.loadPage(1);
                }
            });
        } else {
            this.loadPage(1);
        }
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
