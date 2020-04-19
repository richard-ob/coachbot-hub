import { Component, OnInit } from '@angular/core';
import { TeamStatistics } from '../shared/model/team-statistics.model';
import { TeamService } from '../shared/services/team.service';
import { Router } from '@angular/router';
@Component({
    selector: 'app-team-list',
    templateUrl: './team-list.component.html',
    styleUrls: ['./team-list.component.scss']
})
export class TeamListComponent implements OnInit {

    teamStatistics: TeamStatistics[];
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private teamService: TeamService, private router: Router) {

    }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(page: number) {
        this.teamService.getTeamStatistics(page).subscribe(response => {
            this.teamStatistics = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
        });
    }

    navigatetoProfile(teamId: number) {
        this.router.navigate(['/player-profile', teamId]);
    }

}
