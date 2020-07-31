import { Component, OnInit, Input } from '@angular/core';
import { TeamStatistics } from '../shared/model/team-statistics.model';
import { TeamService } from '../shared/services/team.service';
import { Router } from '@angular/router';
import SortingUtils from '@shared/utilities/sorting-utilities';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { TeamStatisticFilters } from '../shared/model/dtos/paged-team-statistics-request-dto.model';
import EnumUtils from '@shared/utilities/enum-utilities';
import { TeamType } from '../shared/model/team-type.enum';
import { TeamSpotlightStatistic } from './team-spotlight/team-spotlight-statistic.enum';
@Component({
    selector: 'app-team-list',
    templateUrl: './team-list.component.html',
    styleUrls: ['./team-list.component.scss']
})
export class TeamListComponent implements OnInit {

    @Input() tournamentId: number;
    @Input() hideFilters = false;
    teamStatistics: TeamStatistics[];
    filters: TeamStatisticFilters = new TeamStatisticFilters();
    teamTypes = TeamType;
    teamSpotlightStatistic = TeamSpotlightStatistic;
    regionId: number;
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    sortBy: string = null;
    sortOrder = 'ASC';
    timePeriod = 0;
    isLoading = true;
    isLoadingPage = false;
    filtersApplied = false;

    constructor(private teamService: TeamService, private router: Router, private userPreferenceService: UserPreferenceService) { }

    ngOnInit() {
        this.filters.tournamentId = this.tournamentId;
        this.regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.filters.regionId = this.regionId;
        this.loadPage(1);
    }

    loadPage(page: number, sortBy: string = null) {
        this.isLoadingPage = true;
        if (page === this.currentPage) {
            this.sortOrder = SortingUtils.getSortOrder(this.sortBy, sortBy, this.sortOrder);
            this.sortBy = sortBy;
        }
        this.teamService.getTeamStatistics(page, undefined, this.sortBy, this.sortOrder, this.filters)
            .subscribe(response => {
                this.teamStatistics = response.items;
                this.currentPage = response.page;
                this.totalPages = response.totalPages;
                this.totalItems = response.totalItems;
                this.isLoadingPage = false;
                this.isLoading = false;
            });
    }

    setFilters() {
        this.loadPage(this.currentPage);
        this.filtersApplied = true;
    }

    navigatetoProfile(teamId: number) {
        this.router.navigate(['/team-profile', teamId]);
    }

}
