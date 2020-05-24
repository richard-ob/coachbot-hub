import { Component, OnInit } from '@angular/core';
import { TeamStatistics } from '../shared/model/team-statistics.model';
import { TeamService } from '../shared/services/team.service';
import { Router } from '@angular/router';
import SortingUtils from '@shared/utilities/sorting-utilities';
@Component({
    selector: 'app-team-list',
    templateUrl: './team-list.component.html',
    styleUrls: ['./team-list.component.scss']
})
export class TeamListComponent implements OnInit {

    teamStatistics: TeamStatistics[];
    filters;
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    sortBy: string = null;
    sortOrder = 'ASC';
    timePeriod = 0;
    isLoading = true;
    isLoadingPage = false;

    constructor(private teamService: TeamService, private router: Router) { }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(page: number, sortBy: string = null) {
        this.isLoadingPage = true;
        this.sortOrder = SortingUtils.getSortOrder(this.sortBy, sortBy, this.sortOrder);
        this.sortBy = sortBy;
        this.teamService.getTeamStatistics(page, undefined, undefined, undefined, this.sortBy, this.sortOrder).subscribe(response => {
            this.teamStatistics = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
            this.isLoadingPage = false;
        });
    }

    navigatetoProfile(teamId: number) {
        this.router.navigate(['/team-profile', teamId]);
    }

}
