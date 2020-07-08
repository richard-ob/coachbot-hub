import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TeamService } from '../../shared/services/team.service';
import { TeamStatistics } from '../../shared/model/team-statistics.model';
import { TeamProfileSpotlightStatistic } from './team-profile-spotlight/team-profile-spotlight-statistic.enum';

@Component({
    selector: 'app-team-profile-statistics',
    templateUrl: './team-profile-statistics.component.html'
})
export class TeamProfileStatisticsComponent implements OnInit {

    teamId: number;
    teamStatistics: TeamStatistics;
    spotlightStatistic = TeamProfileSpotlightStatistic;
    isLoading = true;

    constructor(private route: ActivatedRoute, private teamService: TeamService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.teamId = +params.get('id');
        });
    }

}
