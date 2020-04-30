import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OverviewType } from 'angular2-calendar-heatmap';
import { TeamService } from '../../shared/services/team.service';
import { TeamStatistics } from '../../shared/model/team-statistics.model';

@Component({
    selector: 'app-team-profile-players',
    templateUrl: './team-profile-players.component.html'
})
export class TeamProfilePlayersComponent implements OnInit {

    teamId: number;
    teamStatistics: TeamStatistics;
    isLoading = true;
    overview = OverviewType.year;

    constructor(private route: ActivatedRoute, private teamService: TeamService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.teamId = +params.get('id');
        });
    }

}
